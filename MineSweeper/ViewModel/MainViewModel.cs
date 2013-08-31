using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

//using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Mono.App.MineSweeper.Model;

namespace Mono.App.MineSweeper.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Size = 10;
            MimeRate = 0.15f;
            BlockSize = 30;
            Initialize();

            Messenger.Default.Register<string>(this, s =>
                {
                    if (s == "fail")
                    {
                        GameState = GameState.GameOver;
                        timer.Dispose();
                    }
                    else if (s == "clear")
                    {
                        GameState = GameState.Clear;
                        timer.Dispose();
                    }
                });
        }

        IDisposable timer;

        private void Initialize()
        {
            if (timer != null)
            {
                timer.Dispose();
            }

            GameState = GameState.Default;
            Rows = Resolver.CreateNewRows(Size, MimeRate);
            var observable = Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
            timer = observable.ObserveOn(SynchronizationContext.Current).Subscribe(v => ElapsedTime = Convert.ToInt32(v));
        }

        //private ICommand startNewCommand;

        //public ICommand StartNewCommand
        //{
        //    get
        //    {
        //        return startNewCommand ??
        //            (startNewCommand = new RelayCommand(() =>
        //            {
        //                Initialize();
        //            }));
        //    }
        //}

        private int elapsedTime;

        public int ElapsedTime
        {
            get { return elapsedTime; }
            set
            {
                elapsedTime = value;
                RaisePropertyChanged("ElapsedTime");
            }
        }

        private IList<BlockRow> rows;

        public IList<BlockRow> Rows
        {
            get { return rows; }
            set
            {
                rows = value;
                RaisePropertyChanged("Rows");
            }
        }

        public int Size { get; set; }

        public float MimeRate { get; set; }

        private ICommand sizeSubmitCommand;

        public ICommand SizeSubmitCommand
        {
            get
            {
                return sizeSubmitCommand ??
                    (sizeSubmitCommand = new RelayCommand(() =>
                  {
                      var ratio = this.Rows.Count / (float)this.Size;
                      //this.Rows = Resolver.CreateNewRows(this.Size, this.MimeRate);
                      this.BlockSize = (int)(BlockSize * ratio);
                      Initialize();
                  }));
            }
        }

        private int blockSize;

        public int BlockSize
        {
            get { return blockSize; }
            set
            {
                blockSize = value;
                RaisePropertyChanged("BlockSize");
            }
        }

        private ICommand sizeChangedCommand;

        public ICommand SizeChangedCommand
        {
            get
            {
                return sizeChangedCommand ??
                    (sizeChangedCommand = new RelayCommand<ItemsControl>((o) =>
                    {
                        SetBlockSize(o);
                    }));
            }
        }

        private ICommand initializeCommand;

        public ICommand InitializeCommand
        {
            get
            {
                return initializeCommand ??
                    (initializeCommand = new RelayCommand<ItemsControl>((o) =>
                    {
#if SILVERLIGHT
#else
                        SetBlockSize(o);

#endif
                    }));
            }
        }

        private void SetBlockSize(ItemsControl o)
        {
            BlockSize = Math.Min((int)o.ActualHeight, (int)o.ActualWidth) / Size;
        }

        private GameState gameState;

        public GameState GameState
        {
            get { return gameState; }
            set
            {
                gameState = value;
                RaisePropertyChanged("GameState");
            }
        }
    }

    public enum GameState
    {
        Default,
        GameOver,
        Clear,
    }
}