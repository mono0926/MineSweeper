using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

//using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Mono.App.MineSweeper.Model
{
    public class Block : ViewModelBase
    {
        public Block(BlockType blockType)
        {
            this.BlockType = blockType;
        }

        public BlockRow Row { get; set; }

        public int ColumnIndex { get { return Row.Blocks.IndexOf(this); } }

        public int RowIndex { get { return Row.RowIndex; } }

        public int Size { get { return Row.Blocks.Count; } }

        public IEnumerable<BlockRow> Rows { get { return Row.Rows; } }

        public IEnumerable<Block> AllBlocks { get { return Rows.SelectMany(x => x.Blocks); } }

        public int MineCount
        {
            get
            {
                var c = AllAngleBlocks.Where(x => x.BlockType == BlockType.Mine).Count();
                return c;
            }
        }

        private bool markedAsMine;

        public bool MarkedAsMine
        {
            get { return markedAsMine; }
            set
            {
                markedAsMine = value;
                RaisePropertyChanged("MarkedAsMine");
            }
        }

        public bool IsCountVisible { get { return this.BlockAppearance == BlockAppearance.Flipped && MineCount != 0; } }

        #region 周りのブロックのプロパティ

        public IEnumerable<Block> AllAngleBlocks { get { return (new Block[] { Left, TopLefft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft }).Where(x => x != null); } }

        public bool IsFlipCandidate
        {
            get
            {
                return BlockType != BlockType.Mine && (FlagType == FlagType.None || FlagType == FlagType.Flag)
                    && AllAngleBlocks.Where(y => y.BlockType == BlockType.Mine).Count() == 0;
            }
        }

        public bool IsCountCandidate
        {
            get
            {
                return BlockType != BlockType.Mine && (FlagType == FlagType.None || FlagType == FlagType.Flag)
                    && AllAngleBlocks.Where(y => y.BlockType == BlockType.Mine).Count() != 0;
            }
        }

        public Block Left
        {
            get
            {
                if (ColumnIndex == 0)
                {
                    return null;//壁
                }
                return Row.Blocks[ColumnIndex - 1];
            }
        }

        public Block TopLefft
        {
            get
            {
                if (ColumnIndex == 0 || RowIndex == 0)
                {
                    return null;//壁
                }
                return Row.Rows[RowIndex - 1].Blocks[ColumnIndex - 1];
            }
        }

        public Block Top
        {
            get
            {
                if (RowIndex == 0)
                {
                    return null;//壁
                }
                return Row.Rows[RowIndex - 1].Blocks[ColumnIndex];
            }
        }

        public Block TopRight
        {
            get
            {
                if (ColumnIndex == Size - 1 || RowIndex == 0)
                {
                    return null;//壁
                }
                return Row.Rows[RowIndex - 1].Blocks[ColumnIndex + 1];
            }
        }

        public Block Right
        {
            get
            {
                if (ColumnIndex == Size - 1)
                {
                    return null;//壁
                }
                return Row.Blocks[ColumnIndex + 1];
            }
        }

        public Block BottomRight
        {
            get
            {
                if (ColumnIndex == Size - 1 || RowIndex == Size - 1)
                {
                    return null;//壁
                }
                return Row.Rows[RowIndex + 1].Blocks[ColumnIndex + 1];
            }
        }

        public Block Bottom
        {
            get
            {
                if (RowIndex == Size - 1)
                {
                    return null;//壁
                }
                return Row.Rows[RowIndex + 1].Blocks[ColumnIndex];
            }
        }

        public Block BottomLeft
        {
            get
            {
                if (ColumnIndex == 0 || RowIndex == Size - 1)
                {
                    return null;//壁
                }
                return Row.Rows[RowIndex + 1].Blocks[ColumnIndex - 1];
            }
        }

        #endregion 周りのブロックのプロパティ

        #region command

        private RelayCommand blockClickCommand;

        public RelayCommand BlockClickCommand
        {
            get
            {
                return blockClickCommand ??
                    (blockClickCommand = new RelayCommand(() =>
                    {
                        //初回
                        if (AllBlocks.Where(x => x.FlagType != FlagType.None).Count() == 0)
                        {
                            var r = new Random();

                            var plain = AllBlocks.Where(x => x.BlockType == BlockType.None);
                            var target = plain.Where(x => x != this).Skip(r.Next(plain.Count() - 1)).Take(1).First();
                            target.BlockType = BlockType.Mine;
                            this.BlockType = BlockType.None;
                            foreach (var b in target.AllAngleBlocks)
                            {
                                b.RaisePropertyChanged("MineCount");
                            }
                        }

                        if (BlockType == BlockType.Mine)
                        {
                            Messenger.Default.Send<string>("fail");
                            FlagType = FlagType.Flip;
                        }
                        else if (FlagType == FlagType.None)
                        {
                            //FlagType = FlagType.Flip;

                            Resolver.CalculateFlipBlock(this);
                        }
                        CheckClear();
                    }, () =>
                    {
                        if (FlagType == FlagType.None)
                        {
                            return true;
                        }
                        return false;
                    }));
            }
        }

        private void CheckClear()
        {
            if (AllBlocks.All(x => (x.BlockType == BlockType.Mine && x.FlagType == FlagType.Mine) || (x.BlockType == BlockType.None && x.FlagType == FlagType.Flip)))
            {
                Messenger.Default.Send<string>("clear");
            }
        }

        private ICommand blockRightClickCommand;

        public ICommand BlockRightClickCommand
        {
            get
            {
                return blockRightClickCommand ??
                    (blockRightClickCommand = new RelayCommand(() =>
                    {
                        switch (FlagType)
                        {
                            case FlagType.None:
                                FlagType = FlagType.Mine;
                                break;
                            case FlagType.Mine:
                                FlagType = FlagType.Flag;
                                break;
                            case FlagType.Flag:
                                FlagType = FlagType.None;
                                break;
                            default:
                                break;
                        }
                        CheckClear();
                    }));
            }
        }

        #endregion command

        // private Visibility isMineVisible;

        public BlockAppearance BlockAppearance
        {
            get
            {
                if (FlagType == FlagType.None)
                {
                    return BlockAppearance.None;
                }
                else if (FlagType == FlagType.Mine)
                {
                    return BlockAppearance.MineMark;
                }
                else if (FlagType == FlagType.Flip)
                {
                    if (BlockType == BlockType.Mine)
                    {
                        return BlockAppearance.Bombed;
                    }
                    return BlockAppearance.Flipped;
                }
                else
                {
                    return BlockAppearance.FlagMark;
                }
            }
        }

        private BlockType blockType;

        public BlockType BlockType
        {
            get { return blockType; }
            set
            {
                blockType = value;
                RaisePropertyChanged("BlockType");
            }
        }

        private FlagType flagType;

        public FlagType FlagType
        {
            get { return flagType; }
            set
            {
                flagType = value;
                RaisePropertyChanged("BlockAppearance");
                RaisePropertyChanged("BlockClickCommand");
                if (value == FlagType.Flip)
                {
                    RaisePropertyChanged("IsCountVisible");
                }
            }
        }
    }

    public enum BlockAppearance
    {
        None,
        Flipped,
        MineMark,
        FlagMark,
        Bombed,
    }

    public enum BlockType
    {
        None,
        Mine,
    }

    public enum FlagType
    {
        None,
        Flag,
        Mine,
        Flip,
    }
}