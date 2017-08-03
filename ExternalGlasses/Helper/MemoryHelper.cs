using ExternalGlasses.Imports;
using ExternalGlasses.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExternalGlasses.Helper
{
    public class MemoryHelper
    {
        private Process _gameProcess;
        private IntPtr _gameHandle;
        private Int32 _clientModule;

        public int ClientModule { get => _clientModule; private set => _clientModule = value; }

        public MemoryHelper()
        {
            _gameProcess = GetGameProcess();
            _gameHandle = GetGameHandle(_gameProcess);
            _clientModule = GetClientModule(_gameProcess);
        }

        ~MemoryHelper()
        {
            Kernel32.CloseHandle(_gameHandle);
        }

        public List<PlayerModel> GetPlayer()
        {
            List<PlayerModel> playerList = new List<PlayerModel>();

            for (int i = 0; i < 64; i++)
            {
                Int32 playerBase = ReadInt32(_clientModule + Offsets.dwEntityList + (i * Offsets.dwEntityListDistance));

                if (playerBase == 0)
                    continue;

                float playerX = ReadFloat(playerBase + Offsets.dwVecOrigin + (0 * 0x4));
                float playerY = ReadFloat(playerBase + Offsets.dwVecOrigin + (1 * 0x4));
                float playerZ = ReadFloat(playerBase + Offsets.dwVecOrigin + (2 * 0x4));

                float playerHeadX = playerX + ReadFloat(playerBase + Offsets.dwVecViewOffset + (0 * 0x4));
                float playerHeadY = playerY + ReadFloat(playerBase + Offsets.dwVecViewOffset + (1 * 0x4));
                float playerHeadZ = playerZ + ReadFloat(playerBase + Offsets.dwVecViewOffset + (2 * 0x4));

                Int32 playerHealth = ReadInt32(playerBase + Offsets.dwHealth);

                Int32 playerTeam = ReadInt32(playerBase + Offsets.dwTeamNumber);

                PlayerModel currentPlayer = new PlayerModel
                {
                    PosX = playerX,
                    PosY = playerY,
                    PosZ = playerZ,
                    HeadX = playerHeadX,
                    HeadY = playerHeadY,
                    HeadZ = playerHeadZ,
                    Health = playerHealth,
                    Team = playerTeam
                };

                playerList.Add(currentPlayer);
            }

            return playerList;
        }

        public float ReadFloat(Int32 offsetPointer)
        {
            byte[] buffer = new byte[sizeof(float)];
            Kernel32.ReadProcessMemory(_gameHandle, offsetPointer, buffer, sizeof(float), out int bytesRead);
            return BitConverter.ToSingle(buffer, 0);
        }

        public Int32 ReadInt32(Int32 offsetPointer)
        {
            byte[] buffer = new byte[sizeof(Int32)];
            Kernel32.ReadProcessMemory(_gameHandle, offsetPointer, buffer, sizeof(Int32), out int bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }

        private Process GetGameProcess()
        {
            Process[] processList = Process.GetProcessesByName("csgo");
            if (processList.Length < 0)
            {
                Console.WriteLine("Unable to find the CS:GO process. Returning an IntPtr.Zero!");
                return null;
            }

            return processList[0];
        }

        private IntPtr GetGameHandle(Process gameProcess)
        {
            return Kernel32.OpenProcess(Kernel32.PROCESS_VM_READ, false, gameProcess.Id);
        }

        private Int32 GetClientModule(Process gameProcess)
        {
            foreach (ProcessModule processModule in gameProcess.Modules)
            {
                if (processModule.ModuleName == "client.dll")
                    return processModule.BaseAddress.ToInt32();
            }

            Console.WriteLine("Unable to find the client.dll in the CS:GO modules. Returning '0'!");
            return 0;
        }
    }
}
