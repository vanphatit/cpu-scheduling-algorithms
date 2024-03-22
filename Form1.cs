using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPU_SCHEDULING_ALGORITHMS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TextBoxStreamWriter writer = new TextBoxStreamWriter(txtConsole);
            Console.SetOut(writer); 
            GanttChartPanel.Invalidate();
            GanttChartPanel.Controls.Clear();
            label2.Hide();
            CountTime.Hide();
            txtConsole.Clear();
            txtConsole.Refresh();
        }

        //=============================================================================================================================
        //Struct
        public struct RGB
        {
            public int x, y, z;
        }
        public struct Process
        {
            public string id;
            public int arrival_time, burst_time, turnaround_time, exit_time, waiting_time, priority, remaining_time, i;
        }
        //=============================================================================================================================


        #region "Event clicked"
        private void fCFSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Algorithm.Text = "FCFS";
            if (dataGridView1.Columns["Priority"] != null)
            {
                dataGridView1.Columns.Remove("Priority");
            }
            RRPanel.Visible = false;
            panel1.Enabled = false;
            Form1_Load(sender, e);
        }

        private void sJFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Algorithm.Text = "SJF";
            if (dataGridView1.Columns["Priority"] != null)
            {
                dataGridView1.Columns.Remove("Priority");
            }
            RRPanel.Visible = false;
            panel1.Enabled = true;
            Form1_Load(sender, e);
        }

        private void rOUNDROBINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Algorithm.Text = "Round Robin";
            if (dataGridView1.Columns["Priority"] != null)
            {
                dataGridView1.Columns.Remove("Priority");
            }
            RRPanel.Visible = true;
            RRPanel.Show();
            panel1.Enabled = false;
            Form1_Load(sender, e);
        }

        private void pRIORITYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Algorithm.Text = "Priority";
            if (dataGridView1.Columns["Priority"] == null)
            {
                dataGridView1.Columns.Add("Priority", "Priority");
            }
            RRPanel.Visible = false;
            panel1.Enabled = true;
            Form1_Load(sender, e);
        }

        private void pRIORITYWITHRRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Algorithm.Text = "Priority with RR";
            if (dataGridView1.Columns["Priority"] == null)
            {
                dataGridView1.Columns.Add("Priority", "Priority");
            }
            RRPanel.Visible = true;
            RRPanel.Show();
            panel1.Enabled = false;
            Form1_Load(sender, e);
        }

        private void GanttChart(Process[] a, int n, int choose)
        {
            CountTime.Text = "" + 0;
            label2.Show();
            CountTime.Show();
            CountTime.Refresh();
            label2.Refresh();

            switch (choose)
            {
                case 1://SJF
                    SJF(a, n);
                    break;
                case 0://SRTF
                    SRTF(a, n);
                    break;
                case 2://FCFS
                    FCFS(a, n);
                    break;
                case 3://Round Robin
                    RoundRobin(a, n);
                    break;
                case 4://Priority NonPreemptive
                    Priority_NonPreemptive(a, n);
                    break;
                case 5://Priority Preemptive
                    Priority_Preemptive(a, n);
                    break;
                case 6://Priority Preemptive
                    Priority_RR(a, n);
                    break;
            }
        }

        private void RunBtn_Click(object sender, EventArgs e)
        {
            Process[] process = new Process[100];
            int flag = 0;

            //Refresh 
            this.Refresh();
            txtConsole.Clear();
            txtConsole.Refresh();
            GanttChartPanel.Controls.Clear();

            //Check type of Algorithm
            if (dataGridView1.RowCount <= 1)
            {
                MessageBox.Show("Please type data before click Run!!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2, MessageBoxOptions.ServiceNotification);
                return;
            }
            if (Algorithm.Text == "SJF")
            {
                if (NonPreemptiveBtn.Checked == false)
                {
                    flag = 0;
                    Console.Write("SJF preemptive");
                }
                else
                {
                    flag = 1;
                    Console.Write("SJF non-preemptive");
                }
            }
            if (Algorithm.Text == "FCFS")
            {
                flag = 2;
                Console.Write("FCFS");
                txtConsole.AppendText(Environment.NewLine);//<=== FIX ERROR: STACK OVERFLOW OF Console.Write();  
            }
            if (Algorithm.Text == "Round Robin")
            {
                flag = 3;
                if (qInput.Value <= 0)
                {
                    MessageBox.Show("Quantum phải lớn hơn 0!!!!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2, MessageBoxOptions.ServiceNotification);
                    return;
                }
                Console.Write("RR");
            }
            if (Algorithm.Text == "Priority")
            {
                if (NonPreemptiveBtn.Checked == true)
                {
                    flag = 4;
                    Console.Write("Priority non-preemptive");
                }
                else
                {
                    flag = 5;
                    Console.Write("Priority preemptive");
                }
            }
            if (Algorithm.Text == "Priority with RR")
            {
                flag = 6;
                if (qInput.Value <= 0)
                {
                    MessageBox.Show("Quantum phải lớn hơn 0!!!!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2, MessageBoxOptions.ServiceNotification);
                    return;
                }
                Console.Write("Priority with RoundRobin");
            }

            //Take data from GridView
            for (int row = 0; row < dataGridView1.Rows.Count; row++)
            {
                for (int col = 0; col < dataGridView1.Rows[row].Cells.Count; col++)
                {
                    if (col == 0)
                    {
                        process[row].id = dataGridView1.Rows[row].Cells[col].Value.ToString();
                    }
                    if (col == 1)
                    {
                        process[row].arrival_time = int.Parse(dataGridView1.Rows[row].Cells[col].Value.ToString());
                    }
                    if (col == 2)
                    {
                        process[row].burst_time = int.Parse(dataGridView1.Rows[row].Cells[col].Value.ToString());
                        process[row].remaining_time = process[row].burst_time;
                        
                    }
                    if (col == 3)
                    {
                        process[row].priority = int.Parse(dataGridView1.Rows[row].Cells[col].Value.ToString());
                    }
                }
                process[row].i = row;
            }

            //Run function
            GanttChart(process, dataGridView1.Rows.Count, flag);
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            Form1_Load(sender, e);
            GanttChartPanel.Controls.Clear();
            GanttChartPanel.Refresh();
            txtConsole.Clear();
            txtConsole.Refresh();
        }

        private void addBtn_Click(object sender, EventArgs e)
        {
            this.Refresh();

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns[1].ReadOnly = false;
            
            dataGridView1.Rows.Insert(dataGridView1.Rows.Count, new object[] { "P" + (dataGridView1.Rows.Count + 1), dataGridView1.Rows.Count, dataGridView1.Rows.Count + 1 , dataGridView1.Rows.Count + 1 });

        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            GanttChartPanel.Refresh();
            dataGridView1.AllowUserToAddRows = false;
            if (dataGridView1.Rows.Count != 2 && dataGridView1.Rows.Count != 0)
            {
                dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 1);
            }
            else
            {
                return;
            }
        }
        #endregion


        #region "Algorithm"
        public static void swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }
        public static void arrangeArrival(Process[] a, int n)
        {
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (a[i].arrival_time > a[j].arrival_time)
                    {
                        swap(ref a[i], ref a[j]);
                    }
                }
            }
        }
        public static void arrangeforPriority(Process[] a, int n)
        {
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (a[i].priority >= a[j].priority && a[i].arrival_time >= a[i].arrival_time)
                    {
                        if(a[i].priority != a[j].priority || a[i].arrival_time != a[i].arrival_time)
                            swap(ref a[i], ref a[j]);
                    }
                }
            }
        }
        public static double averageWaitingTime(Process[] a, int n)
        {
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum += a[i].waiting_time;
            }
            return sum / n;
        }

        // FCFS
        public void FCFS(Process[] a, int n)
        {
            arrangeArrival(a, n);
            Process[] processGantt = new Process[100];

            int current_pos = 0;
            int time = 0;

            for (int i = 0; i < n; i++)
            {
                while (a[i].remaining_time > 0)
                {
                    processGantt[current_pos] = a[i];
                    current_pos++;
                    time++;
                    a[i].remaining_time--;
                }
                if (i == 0)
                {
                    a[i].exit_time = a[i].burst_time;
                }
                else
                {
                    a[i].exit_time = a[i - 1].exit_time + a[i].burst_time;
                }

                a[i].turnaround_time = a[i].exit_time - a[i].arrival_time;
                a[i].waiting_time = a[i].turnaround_time - a[i].burst_time;
            }

            drawProcess(processGantt, n, time);

            //Show result
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("Process ID\tWaiting Time\tTurnaround Time\n");
            for (int i = 0; i < n; i++)
            {
                txtConsole.AppendText(Environment.NewLine);
                Console.Write("{0}\t\t{1}\t\t{2}", a[i].id, a[i].waiting_time, a[i].turnaround_time);
            }
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("\nAverage waiting time: {0}", averageWaitingTime(a, n));
        }
        //====================================================

        //SJF non-preemptive
        public void SJF(Process[] a, int n)
        {
            arrangeArrival(a, n);

            Process[] processGantt = new Process[100];

            int current_pos = 0, currentIndex = 0, lastIndex = 0;
            int time = 0, i = 0;

            for(i = 0; i <= n; i++)
            {
                if(time > 0)
                {
                    a[n+2].burst_time = 999;
                    // find min burst time
                    Process min = a[n+2];
                    int min_pos = currentIndex;
                    for (int j = 0; j < n; j++)
                    {
                        if (a[j].remaining_time > 0 && a[j].remaining_time < min.burst_time)
                        {
                            min = a[j];
                            min_pos = j;
                        }
                    }
                    for (int j = 0; j < n; j++)
                    {
                        if ((a[j].remaining_time == min.remaining_time) && (a[j].arrival_time < min.arrival_time))
                        {
                            min = a[j];
                            min_pos = j;
                        }
                    }
                    currentIndex = min_pos;

                }
                while (a[currentIndex].remaining_time > 0)
                {
                    processGantt[current_pos] = a[currentIndex];
                    current_pos++;
                    time++;
                    a[currentIndex].remaining_time--;

                    if (a[currentIndex].remaining_time == 0)
                    {
                        if (i == 0)
                        {
                            a[currentIndex].exit_time = a[currentIndex].burst_time;
                        }
                        else
                        {
                            a[currentIndex].exit_time = a[lastIndex].exit_time + a[currentIndex].burst_time;
                        }

                        a[currentIndex].turnaround_time = a[currentIndex].exit_time - a[currentIndex].arrival_time;
                        a[currentIndex].waiting_time = a[currentIndex].turnaround_time - a[currentIndex].burst_time;
                        lastIndex = currentIndex;
                    }
                }
            }

            drawProcess(processGantt, n, time);

            txtConsole.AppendText(Environment.NewLine);
            Console.Write("Process ID\tWaiting Time\tTurnaround Time\n");
            for (i = 0; i < n; i++)
            {
                txtConsole.AppendText(Environment.NewLine);
                Console.Write("{0}\t\t{1}\t\t{2}", a[i].id, a[i].waiting_time, a[i].turnaround_time);
            }
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("\nAverage waiting time: {0}", averageWaitingTime(a, n));
        }
        //====================================================
        
        //SRTF
        public void SRTF(Process[] a, int n)
        {
            Process[] processGantt = new Process[100];

            int[] processBurst = new int[100];
            int i, smallest, count = 0, time, end;
            double avg = 0, tt = 0;

            arrangeArrival(a, n);
            for (i = 0; i < n; i++)
                processBurst[i] = a[i].burst_time;

            //---------------------------
            //Calulating..
            processBurst[n+1] = 9999; //Declare Max
            for (time = 0; count != n; time++)
            {
                smallest = n+1;
                for (i = 0; i < n; i++)
                {
                    if (a[i].arrival_time <= time && processBurst[i] < processBurst[smallest] && processBurst[i] > 0)
                    {
                        processGantt[time] = a[i];
                        smallest = i;
                    }
                }
                processBurst[smallest]--;
                if (processBurst[smallest] == 0)
                {
                    count++;

                    end = time + 1;
                    a[smallest].exit_time = end;
                    a[smallest].turnaround_time = end - a[smallest].arrival_time;
                }
            }

            for (i = 0; i < n; i++)
            {
                a[i].waiting_time = a[i].turnaround_time - a[i].burst_time;
            }

            for (i = 0; i < n; i++)
            {
                avg = avg + a[i].waiting_time;
                tt = tt + a[i].turnaround_time;
            }

            drawProcess(processGantt, n, time);
            //Show result
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("Process ID\tWaiting Time\tTurnaround Time\n");
            for (i = 0; i < n; i++)
            {
                txtConsole.AppendText(Environment.NewLine);
                Console.Write("{0}\t\t{1}\t\t{2}", a[i].id, a[i].waiting_time, a[i].turnaround_time);
            }
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("\nAverage waiting time: {0}", averageWaitingTime(a, n));
        }
        //====================================================

        //Round Robin
        public void RoundRobin(Process[] a, int n)
        {
            int quantum = (int)qInput.Value;
            Process[] processGantt = new Process[100];

            int time = 0;

            //---------------------------
            // sort arrvial time
            arrangeArrival(a, n);
            //---------------------------

            //Calulating...
            LinkedList<Process> list = new LinkedList<Process>();//Sử dụng giống Queue :V
            for (int i = 0; i < n; i++)
            {
                list.AddLast(a[i]);
            }

            while (list.Count() > 0)
            {
                Process current = list.First();//check first position

                if (current.remaining_time > quantum)
                {
                    for (int i = 0; i < quantum; i++)
                    {
                        processGantt[time] = current;
                        time++;
                        current.remaining_time--;
                    }
                    list.RemoveFirst();
                    list.AddLast(current);
                }
                else
                {
                    for (int i = 0; i < current.remaining_time; i++)
                    {
                        processGantt[time] = current;
                        time++;
                        current.remaining_time--;
                    }
                    list.RemoveFirst();
                }
                if (current.remaining_time == 0)
                {
                    a[current.i].exit_time = time;
                    a[current.i].turnaround_time = a[current.i].exit_time - a[current.i].arrival_time;
                    a[current.i].waiting_time = a[current.i].turnaround_time - a[current.i].burst_time;
                }
            }
            //---------------------------
            //Drawing...
            drawProcess(processGantt, n, time);
            //Show result
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("Process ID\tWaiting Time\tTurnaround Time\n");
            for (int i = 0; i < n; i++)
            {
                txtConsole.AppendText(Environment.NewLine);
                Console.Write("{0}\t\t{1}\t\t{2}", a[i].id, a[i].waiting_time, a[i].turnaround_time);
            }
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("\nAverage waiting time: {0}", averageWaitingTime(a, n));
        }
        //====================================================

        //Priority NonPreemptive
        public void Priority_NonPreemptive(Process[] a, int n)
        {
            arrangeforPriority(a, n);
            Process[] processGantt = new Process[100];

            int current_pos = 0;
            int time = 0;

            for (int i = 0; i < n; i++)
            {
                while (a[i].remaining_time > 0)
                {
                    processGantt[current_pos] = a[i];
                    current_pos++;
                    time++;
                    a[i].remaining_time--;
                }
                if (i == 0)
                {
                    a[i].exit_time = a[i].burst_time;
                }
                else
                {
                    a[i].exit_time = a[i - 1].exit_time + a[i].burst_time;
                }

                a[i].turnaround_time = a[i].exit_time - a[i].arrival_time;
                a[i].waiting_time = a[i].turnaround_time - a[i].burst_time;
            }

            drawProcess(processGantt, n, time);

            //Show result
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("Process ID\tWaiting Time\tTurnaround Time\n");
            for (int i = 0; i < n; i++)
            {
                txtConsole.AppendText(Environment.NewLine);
                Console.Write("{0}\t\t{1}\t\t{2}", a[i].id, a[i].waiting_time, a[i].turnaround_time);
            }
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("\nAverage waiting time: {0}", averageWaitingTime(a, n));
        }
        //====================================================

        //Priority Preemptive
        public void Priority_Preemptive(Process[] a, int n)
        {
            Process[] processGantt = new Process[100];

            int[] processBurst = new int[100];
            int i, smallest, count = 0, time, end;
            double avg = 0, tt = 0;
            int current_priority;

            arrangeArrival(a, n);
            for (i = 0; i < n; i++)
                processBurst[i] = a[i].burst_time;

            //------------------------------------------
            //Calulating..
            processBurst[n + 1] = 9999; //Declare Max
            for (time = 0; count != n; time++)
            {
                smallest = n + 1;
                current_priority = 9999;
                for (i = 0; i < n; i++)
                {
                    if (current_priority > a[i].priority && a[i].priority > 0)
                    {
                        current_priority = a[i].priority;
                    }
                }

                for (i = 0; i < n; i++)
                {
                    if (a[i].arrival_time <= time && a[i].priority == current_priority && a[i].priority > 0 && processBurst[i] < processBurst[smallest] && processBurst[i] > 0)
                    {
                        processGantt[time] = a[i];
                        smallest = i;
                    }
                }
                processBurst[smallest]--;
                if (processBurst[smallest] == 0)
                {
                    count++;
                    a[smallest].priority = 0;
                    end = time + 1;
                    a[smallest].exit_time = end;
                    a[smallest].turnaround_time = end - a[smallest].arrival_time;
                }
            }

            for (i = 0; i < n; i++)
            {
                a[i].waiting_time = a[i].turnaround_time - a[i].burst_time;
            }

            for (i = 0; i < n; i++)
            {
                avg = avg + a[i].waiting_time;
                tt = tt + a[i].turnaround_time;
            }

            drawProcess(processGantt, n, time);

            txtConsole.AppendText(Environment.NewLine);
            Console.Write("Process ID\tWaiting Time\tTurnaround Time\n");
            for (i = 0; i < n; i++)
            {
                txtConsole.AppendText(Environment.NewLine);
                Console.Write("{0}\t\t{1}\t\t{2}", a[i].id, a[i].waiting_time, a[i].turnaround_time);
            }
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("\nAverage waiting time: {0}", averageWaitingTime(a, n));

        }
        //====================================================

        //Priority with RR
        public void Priority_RR(Process[] a, int n)
        {
            int quantum = (int)qInput.Value;
            Process[] processGantt = new Process[100];

            int time = 0;

            //---------------------------
            // sort arrvial time
            arrangeforPriority(a, n);
            //---------------------------

            //Calulating...
            LinkedList<Process> list = new LinkedList<Process>();//Sử dụng giống Queue :V
            for (int i = 0; i < n; i++)
            {
                list.AddLast(a[i]);
            }

            while (list.Count() > 0)
            {
                Process current = list.First();//check first position

                if (current.remaining_time > quantum)
                {
                    for (int i = 0; i < quantum; i++)
                    {
                        processGantt[time] = current;
                        time++;
                        current.remaining_time--;
                    }
                    list.RemoveFirst();
                    if (list.First.Value.priority <= current.priority)
                        list.AddAfter(list.First, current);
                    else
                        list.AddBefore(list.First, current);
                }
                else
                {
                    int i;
                    for (i = 0; i < current.remaining_time; i++)
                    {
                        processGantt[time] = current;
                        time++;
                    }
                    list.RemoveFirst();
                    current.remaining_time = 0;
                }
                if (current.remaining_time == 0)
                {
                    a[current.i].exit_time = time;
                    a[current.i].turnaround_time = a[current.i].exit_time - a[current.i].arrival_time;
                    a[current.i].waiting_time = a[current.i].turnaround_time - a[current.i].burst_time;
                }
            }

            drawProcess(processGantt, n, time);

            txtConsole.AppendText(Environment.NewLine);
            Console.Write("Process ID\tWaiting Time\tTurnaround Time\n");
            for (int i = 0; i < n; i++)
            {
                txtConsole.AppendText(Environment.NewLine);
                Console.Write("{0}\t\t{1}\t\t{2}", a[i].id, a[i].waiting_time, a[i].turnaround_time);
            }
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("\nAverage waiting time: {0}", averageWaitingTime(a, n));

        }
        //====================================================

        #endregion



        public void drawProcess(Process[] processGantt, int n, int time)
        {
            Font font = new Font("Microsoft Sans Serif", 10);
            RGB[] colorProcess = new RGB[100];
            Random rand = new Random();
            int count = 0;

            //Define color of Process
            for (int i = 0; i < n; i++)
            {
                //Random color
                //Red - Green - Black
                colorProcess[i].x = rand.Next(70, 255);
                colorProcess[i].y = rand.Next(70, 255);
                colorProcess[i].z = rand.Next(50, 255);
            }

            string currentProcess = "";
            int current_pos = -1;

            for (int i = 0; i <= time; i++)
            {
                TextBox txb1 = new TextBox();
                txb1.Location = new Point(count * 20, 2);
                txb1.Multiline = true;
                txb1.Font = font;
                if(currentProcess != processGantt[i].id)
                {
                    currentProcess = processGantt[i].id;
                    txb1.Text = " " + processGantt[i].id;
                    txb1.Text += "\r\n";
                    txb1.Text += "\r\n" + count;
                    current_pos = processGantt[i].i;
                }
                else
                {
                    txb1.Text = " ";
                }
                if(count == time)
                {
                    txb1.ForeColor = Color.White;
                    txb1.Text += "\r\n";
                    txb1.Text += "\r\n" + count;
                    current_pos = n;
                } 
                txb1.Size = new Size(20, 50);
                txb1.BorderStyle = 0;
                txb1.BackColor = Color.FromArgb(255, colorProcess[current_pos].x, colorProcess[current_pos].y, colorProcess[current_pos].z);
                txb1.AutoSize = false;
                txb1.ReadOnly = true;
                txb1.Margin = new Padding(0, 0, 0, 0);
                
                count++;
                GanttChartPanel.Controls.Add(txb1);

                //Count time                
                CountTime.Text = "" + (count - 1);
                CountTime.Refresh();
            }
        }
    }
}
//End~~~