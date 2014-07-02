using System;
using GalaSoft.MvvmLight;

namespace Codesseum.Simulator.ViewModels
{
    public class CellViewModel : ViewModelBase
    {
        public CellViewModel(int x, int y, bool solid)
        {
            X = x;
            Y = y;
            IsSolid = solid;
            BotId = Guid.Empty;
        }

        private int _x;
        public int X
        {
            get { return _x; }
            set { Set(() => X, ref _x, value); }
        }

        private int _y;
        public int Y
        {
            get { return _y; }
            set { Set(() => Y, ref _y, value); }
        }

        private bool _isSolid;
        public bool IsSolid
        {
            get { return _isSolid; }
            set { Set(() => IsSolid, ref _isSolid, value); }
        }

        private bool _isOnBot;
        public bool IsOnBot
        {
            get { return _isOnBot; }
            set { Set(() => IsOnBot, ref _isOnBot, value); }
        }

        private bool _isAttacker;
        public bool IsAttacker
        {
            get { return _isAttacker; }
            set { Set(() => IsAttacker, ref _isAttacker, value); }
        }

        private bool _isAttacked;
        public bool IsAttacked
        {
            get { return _isAttacked; }
            set { Set(() => IsAttacked, ref _isAttacked, value); }
        }

        public Guid BotId { get; set; }
    }
}
