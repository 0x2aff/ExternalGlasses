using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalGlasses.Models
{
    public class PlayerModel
    {
        private float _posX, _posY, _posZ, _headX, _headY, _headZ;
        private float _health;
        private int _team;

        public float PosX { get => _posX; set => _posX = value; }
        public float PosY { get => _posY; set => _posY = value; }
        public float PosZ { get => _posZ; set => _posZ = value; }

        public float Health { get => _health; set => _health = value; }

        public int Team { get => _team; set => _team = value; }

        public float HeadX { get => _headX; set => _headX = value; }
        public float HeadY { get => _headY; set => _headY = value; }
        public float HeadZ { get => _headZ; set => _headZ = value; }
    }
}
