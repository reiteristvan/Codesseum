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
    }
}
