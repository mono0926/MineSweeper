using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mono.App.MineSweeper.Model
{
    public static class Resolver
    {
        public static IEnumerable<Block> CalculateFlipBlock(Block block)
        {
            var result = new List<Block>();
            block.FlagType = FlagType.Flip;
            foreach (var b in block.AllAngleBlocks.Where(x => x.IsCountCandidate))
            {
                b.FlagType = FlagType.Flip;
            }
            result.Add(block);
            foreach (var b in block.AllAngleBlocks.Where(x => x.IsFlipCandidate))
            {
                result.AddRange(CalculateFlipBlock(b));
            }
            return result;
        }

        public static IList<BlockRow> CreateNewRows(int size, float mimeRate)
        {
            var total = size * size;
            var mineNum = total * mimeRate;
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
                    blocks.Add(new Block(BlockType.Mine));
                }
                else
                {
                    blocks.Add(new Block(BlockType.None));
                }
            }

            var rows = new List<BlockRow>();
            for (var i = 0; i < size; i++)
            {
                var rowBlocks = blocks.Skip(i * size).Take(size);
                var blockRow = new BlockRow { Blocks = rowBlocks.ToList(), };
                foreach (var b in rowBlocks)
                {
                    b.Row = blockRow;
                }
                rows.Add(blockRow);
                blockRow.Rows = rows;
            }

            return rows;
        }
    }
}