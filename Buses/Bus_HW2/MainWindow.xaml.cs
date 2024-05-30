using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bus_HW2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            stations[0] = "A";
            stations[1] = "B";
            stations[2] = "C";
            stations[3] = "D";
            buses = new List<string>
            {
                "148"
            };
            busesPeople = new List<int>
            {
                0
            };
            statIn = 1;
            peopIn = 1;
        }

        private string[] stations = new string[4];
        private int statIn;
        private int[] peoples = new int[4];
        private int peopIn;
        private List<string> buses;
        private List<int> busesPeople;
        private Thread thread1;
        private Thread thread2;
        private Thread thread3;


        private void PB_prog()
        {
            while (true) 
            {
                Dispatcher.Invoke(() => pb.Value++);
                Thread.Sleep(50);
            }
        }

        private void AddPeople()
        {
            while (true)
            {
                for(int i = 0; i < Dispatcher.Invoke(() => peoples.Length); i++)
                {
                    Dispatcher.Invoke(() => peoples[i] += new Random().Next(0, 5));
                    Thread.Sleep(50);
                }
                Thread.Sleep(1000);
            }
        }

        private void NextStation()
        {
            while (true)
            {
                if (Dispatcher.Invoke(() => pb.Value) >= 100)
                {
                    Dispatcher.Invoke(() => pb.Value = 0);
                    Dispatcher.Invoke(() => statIn++);
                    if (Dispatcher.Invoke(() => statIn) == 4)
                    {
                        Dispatcher.Invoke(() => statIn = 0);
                    }
                    Dispatcher.Invoke(() => peopIn++);
                    if (Dispatcher.Invoke(() => peopIn) == 4)
                    {
                        Dispatcher.Invoke(() => peopIn = 0);
                    }
                    Dispatcher.Invoke(() => tblockNextST.Text = Dispatcher.Invoke(() => stations[statIn]));
                    Parallel.Invoke(Peresadka);
                }
                Dispatcher.Invoke(() => tblockNextP.Text = Dispatcher.Invoke(() => peoples[peopIn].ToString()));
            }
        }

        private void Peresadka()
        {
            for (int i = 0; i < buses.Count; i++)
            {
                Dispatcher.Invoke(() => busesPeople[i] -= new Random().Next(0, busesPeople[i]));
                int temp = new Random().Next(0, Dispatcher.Invoke(() => peoples[peopIn - 1 == -1? 3 : peopIn - 1]));
                if(temp + Dispatcher.Invoke(() => busesPeople[i]) > 48)
                {
                    while (temp + Dispatcher.Invoke(() => busesPeople[i]) > 48)
                    {
                        i--;
                    }
                }
                Dispatcher.Invoke(() => busesPeople[i] += temp);
                Dispatcher.Invoke(() => peoples[peopIn - 1 == -1 ? 3 : peopIn - 1] -= temp);
            }
        }

        private void btStop_Click(object sender, RoutedEventArgs e)
        {
            if (thread1 != null)
            {
                if (thread1.ThreadState == ThreadState.Running || thread1.ThreadState == ThreadState.WaitSleepJoin)
                {
                    thread1.Suspend();
                    thread2.Suspend();
                    thread3.Suspend();
                }
            }
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            if(thread1 == null)
            { 
                thread1 = new Thread(PB_prog);
                thread1.Start();
                thread2 = new Thread(AddPeople);
                thread2.Start();  
                thread3 = new Thread(NextStation);
                thread3.Start();
            }
            else
            {
                if(thread1.ThreadState != ThreadState.WaitSleepJoin)
                {
                    thread1.Resume();
                    thread2.Resume();
                    thread3.Resume();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(thread1.ThreadState == ThreadState.Suspended)
            {
                thread1.Resume();
                thread2.Resume();
                thread3.Resume();
            }
            thread1.Abort();
            thread2.Abort();
            thread3.Abort();
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            buses.Add(tbox.Text);
            busesPeople.Add(0);
            BusC.Text = buses.Count.ToString();
        }
    }
}
