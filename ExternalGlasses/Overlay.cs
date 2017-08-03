using ExternalGlasses.Helper;
using ExternalGlasses.Models;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Factory = SharpDX.Direct2D1.Factory;
using FontFactory = SharpDX.DirectWrite.Factory;

namespace ExternalGlasses
{
    public partial class Overlay : Form
    {
        private WindowRenderTarget _device;
        private HwndRenderTargetProperties _renderProperties;
        private SolidColorBrush _solidColorBrush;
        private Factory _factory;

        private SolidColorBrush _boxBrush;

        // Fonts
        private TextFormat _font, _fontSmall;
        private FontFactory _fontFactory;
        private const string _fontFamily = "Arial";
        private const float _fontSize = 12.0f;
        private const float _fontSizeSmall = 10.0f;

        private IntPtr _handle;
        private Thread _threadDx = null;

        private float[] _viewMatrix = new float[16];
        private Vector3 _worldToScreenPos = new Vector3();

        private bool _running = false;

        public Overlay()
        {
            _handle = Handle;
            InitializeComponent();
        }

        private void LoadOverlay(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);

            _factory = new Factory();
            _fontFactory = new FontFactory();
            _renderProperties = new HwndRenderTargetProperties
            {
                Hwnd = Handle,
                PixelSize = new SharpDX.Size2(Size.Width, Size.Height),
                PresentOptions = PresentOptions.None
            };

            // Initialize DirectX
            _device = new WindowRenderTarget(_factory, new RenderTargetProperties(new PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)), _renderProperties);
            _solidColorBrush = new SolidColorBrush(_device, new RawColor4(Color.White.R, Color.White.G, Color.White.B, Color.White.A));

            _boxBrush = new SolidColorBrush(_device, new RawColor4(255, 0, 0, 0.5f));

            // Initialize Fonts
            _font = new TextFormat(_fontFactory, _fontFamily, _fontSize);
            _fontSmall = new TextFormat(_fontFactory, _fontFamily, _fontSizeSmall);

            _threadDx = new Thread(new ParameterizedThreadStart(DirectXThread))
            {
                Priority = ThreadPriority.Highest,
                IsBackground = true
            };

            _running = true;
            _threadDx.Start();
        }

        private void ClosedOverlay(object sender, FormClosedEventArgs e)
        {
            _running = false;
        }

        private void DirectXThread(object sender)
        {
            List<PlayerModel> currentPlayerList = new List<PlayerModel>();
            MemoryHelper memoryHelper = new MemoryHelper();

            float posX = 0;
            float posY = 0;

            float headX = 0;
            float headY = 0;

            float height = 0;
            float width = 0;

            while (_running)
            {
                _device.BeginDraw();
                _device.Clear(new RawColor4(Color.Transparent.R, Color.Transparent.G, Color.Transparent.B, Color.Transparent.A));
                _device.TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode.Aliased;

                // Render our ESP here
                currentPlayerList = memoryHelper.GetPlayer();

                Int32 localPlayer = memoryHelper.ReadInt32(memoryHelper.ClientModule + Offsets.dwLocalPlayer);
                Int32 localTeam = memoryHelper.ReadInt32(localPlayer + Offsets.dwTeamNumber);

                for (int i = 0; i < 16; i++)
                    _viewMatrix[i] = memoryHelper.ReadFloat(memoryHelper.ClientModule + Offsets.dwViewMatrix + (i * 0x4));

                foreach (var player in currentPlayerList)
                {
                    Vector3 entityPosition = new Vector3
                    {
                        X = player.PosX,
                        Y = player.PosY,
                        Z = player.PosZ
                    };

                    Vector3 booleanVector = new Vector3();
                    if (WorldToScreen(entityPosition, booleanVector))
                    {
                        posX = _worldToScreenPos.X;
                        posY = _worldToScreenPos.Y;

                        Vector3 entityHeadPosition = new Vector3
                        {
                            X = player.HeadX,
                            Y = player.HeadY,
                            Z = player.HeadZ
                        };

                        if (WorldToScreen(entityHeadPosition, booleanVector))
                        {
                            headX = _worldToScreenPos.X;
                            headY = _worldToScreenPos.Y - _worldToScreenPos.Y / 64;

                            if (player.Team != localTeam)
                            {
                                if (player.Health > 0)
                                {
                                    height = posY - headY;
                                    width = height / 2;

                                    float[] a = new float[2];
                                    a[0] = 1;
                                    a[1] = 2;

                                    _device.DrawLine(new RawVector2(posX - width / 2, posY), new RawVector2(posX - width / 2, headY), _boxBrush, 0.5f); // Left
                                    _device.DrawLine(new RawVector2(posX - width / 2, headY), new RawVector2(headX + width / 2, headY), _boxBrush, 0.5f); // Top
                                    _device.DrawLine(new RawVector2(headX + width / 2, headY), new RawVector2(posX + width / 2, posY), _boxBrush, 0.5f); // Right
                                    _device.DrawLine(new RawVector2(posX + width / 2, posY), new RawVector2(posX - width / 2, posY), _boxBrush, 0.5f); // Bottom
                                }
                            }
                        }
                    }
                }

                // End Render of our ESP

                _device.EndDraw();
            }
        }

        bool WorldToScreen(Vector3 from, Vector3 to)
        {
            float w = 0.0f;

            to.X = _viewMatrix[0] * from.X + _viewMatrix[1] * from.Y + _viewMatrix[2] * from.Z + _viewMatrix[3];
            to.Y = _viewMatrix[4] * from.X + _viewMatrix[5] * from.Y + _viewMatrix[6] * from.Z + _viewMatrix[7];

            w = _viewMatrix[12] * from.X + _viewMatrix[13] * from.Y + _viewMatrix[14] * from.Z + _viewMatrix[15];

            if (w < 0.01f)
                return false;

            to.X *= (1.0f / w);
            to.Y *= (1.0f / w);

            int width = Size.Width;
            int height = Size.Height;

            float x = width / 2;
            float y = height / 2;

            x += 0.5f * to.X * width + 0.5f;
            y -= 0.5f * to.Y * height + 0.5f;

            to.X = x;
            to.Y = y;

            _worldToScreenPos.X = to.X;
            _worldToScreenPos.Y = to.Y;

            return true;
        }

        protected override bool ShowWithoutActivation => true;

        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOPMOST = 0x00000008;

        protected override CreateParams CreateParams
        {
            get
            {
                var param = base.CreateParams;
                param.ExStyle |= WS_EX_TOPMOST;
                param.ExStyle |= WS_EX_NOACTIVATE;
                return param;
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr handle);

        private const int WM_ACTIVATE = 6;
        private const int WA_INACTIVE = 0;

        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_NOACTIVATEANDEAT = 0x0004;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = (IntPtr)MA_NOACTIVATEANDEAT;
                return;
            }
            if (m.Msg == WM_ACTIVATE) 
            {
                if (((int)m.WParam & 0xFFFF) != WA_INACTIVE)
                    if (m.LParam != IntPtr.Zero)
                        SetActiveWindow(m.LParam);
                    else
                        SetActiveWindow(IntPtr.Zero);
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
