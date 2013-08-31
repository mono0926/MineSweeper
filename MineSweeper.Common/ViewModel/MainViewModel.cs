using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Mono.App.MineSweeper.Model;

namespace Mono.App.MineSweeper.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Size = 10;

            Rows = InitializeRows(Size);
        }

        private static IEnumerable<BlockRow> InitializeRows(int size)
        {
            var total = size * size;
            var mineNum = total * 0.2;
            var random = new Random();
            var mineIndixe = new List<int>();
            for (var i = 0; i < mineNum; i++)
            {
                while (true)
                {
                    var index = random.Next(total + 1);
                    if (!mineIndixe.Contains(index))
                    {
                        mineIndixe.Add(index);
                        break;
                    }
                }
            }

            var blocks = new List<Block>();
            for (var i = 0; i < total; i++)
            {
                if (mineIndixe.Contains(i))
                {
                    blocks.Add(new Block { Type = BlockType.Mine });
                }
                else
                {
                    blocks.Add(new Block { Type = BlockType.Plain });
                }
            }

            var rows = new List<BlockRow>();
            for (var i = 0; i < size; i++)
            {
                rows.Add(new BlockRow
                {
                    Blocks = blocks.Skip(i *
                        size).Take(size)
                });
            }

            return rows;
        }

        private IEnumerable<BlockRow> rows;

        public IEnumerable<BlockRow> Rows
        {
            get { return rows; }
            set
            {
                rows = value;
                RaisePropertyChanged("Rows");
            }
        }

        public int Size { get; set; }

        private ICommand sizeSubmitCommand;

        public ICommand SizeSubmitCommand
        {
            get
            {
                return sizeSubmitCommand ??
                    (sizeSubmitCommand = new RelayCommand(() =>
                  {
                      this.Rows = InitializeRows(this.Size);
                  }));
            }
        }
    }
}