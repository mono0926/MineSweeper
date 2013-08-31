using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Mono.App.MineSweeper.Model
{
    public class Block : ViewModelBase
    {
        private BlockType type;

        public BlockType Type
        {
            get { return type; }
            set
            {
                type = value;
                RaisePropertyChanged("Type");
                RaisePropertyChanged("Color");
            }
        }

        public string Color
        {
            get
            {
                switch (Type)
                {
                    case BlockType.Plain:
                        return "Azure";
                    case BlockType.Flipped:
                        return "Yellow";
                    case BlockType.Mine:
                        return "Red";
                    case BlockType.Flagged:
                        return "Blue";
                    default:
                        return "White";
                }
            }
        }

        private ICommand blockClickCommand;

        public ICommand BlockClickCommand
        {
            get
            {
                return blockClickCommand ??
                    (blockClickCommand = new RelayCommand(() =>
                    {
                        switch (Type)
                        {
                            case BlockType.Plain:
                                Type = BlockType.Flipped;
                                break;
                            case BlockType.Flipped:
                                //Type = BlockType.Flipped;
                                break;
                            case BlockType.Mine:
                                Type = BlockType.Flagged;
                                break;
                            case BlockType.Flagged:
                                //Type = BlockType.Flipped;
                                break;
                            default:
                                break;
                        }
                    }));
            }
        }
    }

    public enum BlockType
    {
        Plain,//not mine, not flipped
        Flipped,//not mine, flipped
        Mine,//mine, not flipped
        Flagged,//flagged
    }
}