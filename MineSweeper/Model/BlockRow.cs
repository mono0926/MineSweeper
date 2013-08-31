using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Mono.App.MineSweeper.Model
{
    public class BlockRow : ViewModelBase
    {
        public IList<BlockRow> Rows { get; set; }

        public IList<Block> Blocks { get; set; }

        public int RowIndex { get { return Rows.IndexOf(this); } }
    }
}