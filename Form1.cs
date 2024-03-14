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
            //RRPanel.Visible = false;
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
            public int arrival_time, burst_time, turnaround_time, exit_time, waiting_time, service_time, priority, remaining_time, i;
        }
        //=============================================================================================================================

        private void fCFSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Algorithm.Text = "FCFS";
            RRPanel.Visible = false;
            panel1.Enabled = false;
            Form1_Load(sender, e);
        }

        private void sJFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Algorithm.Text = "SJF";
            RRPanel.Visible = false;
            panel1.Enabled = true;
            Form1_Load(sender, e);
        }

        private void GanttChart(Process[] a, int n, int choose)
        {
            Graphics g = GanttChartPanel.CreateGraphics();
            Pen p = new Pen(Color.Black);
            Font f = new Font("Microsoft Sans Serif", 10);

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

            }
        }

        private void RunBtn_Click(object sender, EventArgs e)
        {
            Process[] process = new Process[100];
            int i = 0;
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
            

            //Take data from GridView
            
            for (int row = 0; row < dataGridView1.Rows.Count; row++)
            {
                for (int col = 0; col < dataGridView1.Rows[row].Cells.Count; col++)
                {
                        if (col == 0)
                        {
                            process[i].id = dataGridView1.Rows[row].Cells[col].Value.ToString();
                        }
                        if (col == 1)
                        {
                            process[i].arrival_time = int.Parse(dataGridView1.Rows[row].Cells[col].Value.ToString());
                        }
                        if (col == 2)
                        {
                            process[i++].burst_time = process[i].remaining_time = int.Parse(dataGridView1.Rows[row].Cells[col].Value.ToString());
                        }
                }
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
            
            dataGridView1.Rows.Insert(dataGridView1.Rows.Count, new object[] { "P" + (dataGridView1.Rows.Count + 1), dataGridView1.Rows.Count, dataGridView1.Rows.Count + 1 });

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

        //=============================================================================================================================
        //SJF preemptive
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
        public static void completionTime(Process[] a, int n)
        {
            int temp, min_idx = -1;
            //Calculate first process
            a[0].exit_time = a[0].arrival_time + a[0].burst_time;
            a[0].turnaround_time = a[0].exit_time - a[0].arrival_time;
            a[0].waiting_time = a[0].turnaround_time - a[0].burst_time;
            //Calculate all process left
            for (int i = 1; i < n; i++)
            {
                temp = a[i - 1].exit_time;
                int low = a[i].burst_time;
                //Find min
                for (int j = i; j < n; j++)
                {
                    if (temp >= a[j].arrival_time && low >= a[j].burst_time)
                    {
                        low = a[j].burst_time;
                        min_idx = j;
                    }
                }
                //Calculate min process
                a[min_idx].exit_time = temp + a[min_idx].burst_time;
                a[min_idx].turnaround_time = a[min_idx].exit_time - a[min_idx].arrival_time;
                a[min_idx].waiting_time = a[min_idx].turnaround_time - a[min_idx].burst_time;
                //Swap
                swap(ref a[min_idx], ref a[i]);
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
        public static void sortPID(Process[] a, int n)
        {
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    int compare = String.Compare(a[i].id, a[j].id);
                    if (compare > 0)
                    {
                        swap(ref a[i], ref a[j]);
                    }
                }
            }
        }

        //sort Process by arrival time
        public static void sortArrivalTime(Process[] a, int n)
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

        //sort Process by burst time
        public static void sortBurstTime(Process[] a, int n, int[] x)
        {
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (a[i].burst_time > a[j].burst_time)
                    {
                        swap(ref a[i], ref a[j]);
                    }
                }
            }

            for (int i = 0; i < n; i++)
                x[i] = a[i].burst_time;
        }

        public void SJF(Process[] a, int n)
        {
            Random rand = new Random();
            RGB[] colorProcess = new RGB[100];
            int xx, yy, zz, i;
            arrangeArrival(a, n);
            completionTime(a, n);

            for (i = 0; i < n; i++)
            {
                //Random color 
                xx = rand.Next(50, 255);
                yy = rand.Next(50, 255);
                zz = rand.Next(50, 255);

                colorProcess[i].x = xx;
                colorProcess[i].y = yy;
                colorProcess[i].z = zz;
            }

            for (i = 0; i < n; i++)
            {
                a[i].turnaround_time = a[i].exit_time - a[i].arrival_time;
                a[i].waiting_time = a[i].turnaround_time - a[i].burst_time;
            }
            int count = 0, remain = 0;
            Font font = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular);
            for (i = 0; i < n; i++)
            {

                TextBox txb1 = new TextBox();
                if (a[i].burst_time == 1)
                {
                    txb1.Location = new Point(count * 20, 2);
                    txb1.Multiline = true;
                    txb1.Font = font;
                    txb1.Text = " " + a[i].id;
                    txb1.Text += "\r\n";
                    txb1.Text += "\r\n" + count;
                    txb1.BorderStyle = 0;
                    txb1.BackColor = System.Drawing.Color.FromArgb(255, colorProcess[i].x, colorProcess[i].y, colorProcess[i].z);
                    txb1.AutoSize = false;
                    txb1.ReadOnly = true;
                    txb1.Margin = new Padding(0, 0, 0, 0);
                    txb1.Size = new Size(19, 50);
                    GanttChartPanel.Controls.Add(txb1);
                    count++;
                }
                else
                {
                    remain = a[i].burst_time;
                    txb1.Location = new Point(count * 20, 2);
                    txb1.Multiline = true;
                    txb1.Font = font;
                    txb1.Text = " " + a[i].id;
                    txb1.Text += "\r\n";
                    txb1.Text += "\r\n" + count;
                    txb1.BorderStyle = 0;
                    txb1.BackColor = System.Drawing.Color.FromArgb(255, colorProcess[i].x, colorProcess[i].y, colorProcess[i].z);
                    txb1.AutoSize = false;
                    txb1.ReadOnly = true;
                    txb1.Margin = new Padding(0, 0, 0, 0);
                    txb1.Size = new Size(20, 50);
                    GanttChartPanel.Controls.Add(txb1);
                    count++;
                    remain--;
                }
                while (remain != 0)
                {
                    if (remain == 1)
                    {
                        TextBox txb = new TextBox();
                        txb.Location = new Point(count * 20, 2);
                        txb.Multiline = true;
                        txb.Font = font;
                        txb.Text = " ";
                        txb.BorderStyle = 0;
                        txb.BackColor = System.Drawing.Color.FromArgb(255, colorProcess[i].x, colorProcess[i].y, colorProcess[i].z);
                        txb.AutoSize = false;
                        txb.ReadOnly = true;
                        txb.Margin = new Padding(0, 0, 0, 0);
                        txb.Size = new Size(19, 50);
                        GanttChartPanel.Controls.Add(txb);
                        count++;
                        remain--;
                    }
                    else
                    {
                        TextBox txb = new TextBox();
                        txb.Location = new Point(count * 20, 2);
                        txb.Multiline = true;
                        txb.Font = font;
                        txb.Text = " ";
                        txb.BorderStyle = 0;
                        txb.BackColor = System.Drawing.Color.FromArgb(255, colorProcess[i].x, colorProcess[i].y, colorProcess[i].z);
                        txb.AutoSize = false;
                        txb.ReadOnly = true;
                        txb.Margin = new Padding(0, 0, 0, 0);
                        txb.Size = new Size(20, 50);
                        GanttChartPanel.Controls.Add(txb);
                        count++;
                        remain--;
                    }
                    //Count time                
                    CountTime.Text = "" + count;
                    CountTime.Refresh();
                }
                //Count time                
                CountTime.Text = "" + count;
                CountTime.Refresh();
            }

            sortPID(a, n);
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("Process ID\tWaiting Time\tTurnaround Time\n");
            for (i = 0; i < n; i++)
            {
                txtConsole.AppendText(Environment.NewLine);
                Console.Write("{0}\t\t{1}\t\t{2}", a[i].id, a[i].waiting_time, a[i].turnaround_time); ;
            }
            txtConsole.AppendText(Environment.NewLine);
            Console.Write("\nAverage waiting time: {0}", averageWaitingTime(a, n));

        }
        //====================================================
        //SJF non-preemptive
        public void SRTF(Process[] a, int n)
        {
            Random rand = new Random();
            Process[] processGantt = new Process[100];
            RGB[] colorStandar = new RGB[100];
            RGB[] colorProcess = new RGB[100];
            int xx, yy, zz;

            int[] x = new int[100];
            int i, smallest, count = 0, time, end;
            double avg = 0, tt = 0;

            //Define color of Process
            for (i = 0; i < n; i++)
            {
                //Random color
                xx = rand.Next(70, 255);
                yy = rand.Next(70, 255);
                zz = rand.Next(50, 255);
                //Red - Green - Black
                colorStandar[i].x = xx;
                colorStandar[i].y = yy;
                colorStandar[i].z = zz;
            }

            sortArrivalTime(a, n);
            for (i = 0; i < n; i++)
                x[i] = a[i].burst_time;

            //---------------------------
            //Calulating..
            x[n+1] = 9999; //Declare Max
            Process currentProcessRunning;
            for (time = 0; count != n; time++)
            {
                smallest = n+1;
                for (i = 0; i < n; i++)
                {
                    if (a[i].arrival_time <= time && x[i] < x[smallest] && x[i] > 0)
                    {
                        processGantt[time] = a[i];
                        colorProcess[time] = colorStandar[i];
                        smallest = i;
                    }
                }
                x[smallest]--;
                if (x[smallest] == 0)
                {
                    count++;

                    end = time + 1;
                    a[smallest].exit_time = end;
                    a[smallest].turnaround_time = end - a[smallest].arrival_time;

                    //sortBurstTime(a, n, x);
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
            //---------------------------
            //Drawing...
            label2.Visible = true;
            CountTime.Visible = true;
            count = 0;
            Font font = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular);

            for (i = 0; i <= time; i++)
            {

                if (i == time)
                {
                    TextBox txb = new TextBox();
                    txb.Location = new Point(i * 20, 2);
                    txb.Multiline = true;
                    txb.Font = font;
                    txb.Text = string.Format(" ");
                    txb.Text += "\r\n";
                    txb.Text += "\r\n" + i;
                    txb.BorderStyle = 0;
                    txb.BackColor = System.Drawing.Color.FromArgb(255, 60, 90, 190);
                    txb.AutoSize = false;
                    txb.ReadOnly = true;
                    txb.Margin = new Padding(0, 0, 0, 0);
                    txb.Size = new Size(15, 50);
                    GanttChartPanel.Controls.Add(txb);
                }
                else
                {
                    if (i == 0)
                    {
                        drawProcess(processGantt, colorProcess, font, i, i, 20, 50);
                    }
                    else
                    {
                        if (processGantt[i].id != processGantt[i - 1].id)
                        {
                            drawProcess(processGantt, colorProcess, font, i, i, 20, 50);
                        }
                        else
                        {
                            TextBox txb = new TextBox();
                            txb.Location = new Point(i * 20, 2);
                            txb.Multiline = true;
                            txb.Font = font;
                            txb.Text = string.Format(" ");
                            txb.BorderStyle = 0;
                            txb.BackColor = System.Drawing.Color.FromArgb(255, colorProcess[i].x, colorProcess[i].y, colorProcess[i].z);
                            txb.AutoSize = false;
                            txb.ReadOnly = true;
                            txb.Margin = new Padding(0, 0, 0, 0);
                            txb.Size = new Size(20, 50);
                            GanttChartPanel.Controls.Add(txb);
                        }
                    }
                }

                //Count time                
                CountTime.Text = "" + i;
                CountTime.Refresh();

            }
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

        public void FCFS(Process[] a, int n)
        {
            arrangeArrival(a, n);

            // waiting time for first process is 0
            a[0].service_time = a[0].arrival_time;
            a[0].waiting_time = 0;

            // calculating waiting time
            for (int i = 1; i < n; i++)
            {
                a[i].service_time = a[i - 1].service_time + a[i - 1].burst_time;
                a[i].waiting_time = a[i].service_time - a[i].arrival_time;
                if (a[i].waiting_time < 0)
                {
                    a[i].waiting_time = 0;
                }
            }
            // calculating turnaround time 
            for (int i = 0; i < n; i++)
            {
                a[i].turnaround_time = a[i].burst_time + a[i].waiting_time;
            }

            drawProcess(a, n);

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
        public void drawProcess(Process[] a, int n)
        {
            Font font = new Font("Microsoft Sans Serif", 10);
            RGB[] colorProcess = new RGB[100];
            Random rand = new Random();
            int count = 0, i, remain = 0;
            //Define color of Process
            for (i = 0; i < n; i++)
            {
                //Random color
                //Red - Green - Black
                colorProcess[i].x = rand.Next(70, 255);
                colorProcess[i].y = rand.Next(70, 255);
                colorProcess[i].z = rand.Next(50, 255);
            }
            for (i = 0; i < n; i++)
            {

                TextBox txb1 = new TextBox();
                if (a[i].burst_time == 1)
                {
                    txb1.Location = new Point(count * 20, 2);
                    txb1.Multiline = true;
                    txb1.Font = font;
                    txb1.Font = font;
                    txb1.Text = " " + a[i].id;
                    txb1.Text += "\r\n";
                    txb1.Text += "\r\n" + count;
                    txb1.BorderStyle = 0;
                    txb1.BackColor = System.Drawing.Color.FromArgb(255, colorProcess[i].x, colorProcess[i].y, colorProcess[i].z);
                    txb1.AutoSize = false;
                    txb1.ReadOnly = true;
                    txb1.Margin = new Padding(0, 0, 0, 0);
                    txb1.Size = new Size(19, 50);
                    GanttChartPanel.Controls.Add(txb1);
                    count++;
                }
                else
                {
                    remain = a[i].burst_time;
                    txb1.Location = new Point(count * 20, 2);
                    txb1.Multiline = true;
                    txb1.Font = font;
                    txb1.Text = " " + a[i].id;
                    txb1.Text += "\r\n";
                    txb1.Text += "\r\n" + count;
                    txb1.BorderStyle = 0;
                    txb1.BackColor = System.Drawing.Color.FromArgb(255, colorProcess[i].x, colorProcess[i].y, colorProcess[i].z);
                    txb1.AutoSize = false;
                    txb1.ReadOnly = true;
                    txb1.Margin = new Padding(0, 0, 0, 0);
                    txb1.Size = new Size(20, 50);
                    GanttChartPanel.Controls.Add(txb1);
                    count++;
                    remain--;
                }
                while (remain != 0)
                {
                    if (remain == 1)
                    {
                        TextBox txb = new TextBox();
                        txb.Location = new Point(count * 20, 2);
                        txb.Multiline = true;
                        txb.Font = font;
                        txb.Text = " ";
                        txb.BorderStyle = 0;
                        txb.BackColor = System.Drawing.Color.FromArgb(255, colorProcess[i].x, colorProcess[i].y, colorProcess[i].z);
                        txb.AutoSize = false;
                        txb.ReadOnly = true;
                        txb.Margin = new Padding(0, 0, 0, 0);
                        txb.Size = new Size(19, 50);
                        GanttChartPanel.Controls.Add(txb);
                        count++;
                        remain--;
                    }
                    else
                    {
                        TextBox txb = new TextBox();
                        txb.Location = new Point(count * 20, 2);
                        txb.Multiline = true;
                        txb.Font = font;
                        txb.Text = " ";
                        txb.BorderStyle = 0;
                        txb.BackColor = System.Drawing.Color.FromArgb(255, colorProcess[i].x, colorProcess[i].y, colorProcess[i].z);
                        txb.AutoSize = false;
                        txb.ReadOnly = true;
                        txb.Margin = new Padding(0, 0, 0, 0);
                        txb.Size = new Size(20, 50);
                        GanttChartPanel.Controls.Add(txb);
                        count++;
                        remain--;
                    }
                    //Count time                
                    CountTime.Text = "" + count;
                    CountTime.Refresh();
                }
                //Count time                
                CountTime.Text = "" + count;
                CountTime.Refresh();
            }
        }
        public void drawProcess(Process[] processGantt, RGB[] colorProcess, Font font, int i, int k, int sizex, int sizey)
        {
            TextBox txb = new TextBox();
            txb.Location = new Point(k * 20, 2);
            txb.Name = string.Format("P{0}", k);
            txb.Tag = string.Format("[{0}]", k);
            txb.Multiline = true;
            txb.BorderStyle = 0;
            txb.Font = font;
            txb.Text = string.Format(processGantt[i].id);
            txb.Text += "\r\n";
            txb.Text += "\r\n" + i;
            txb.BackColor = System.Drawing.Color.FromArgb(255, colorProcess[i].x, colorProcess[i].y, colorProcess[i].z);
            txb.AutoSize = false;
            txb.ReadOnly = true;
            txb.Margin = new Padding(0, 0, 0, 0);
            txb.Size = new Size(sizex, sizey);
            GanttChartPanel.Controls.Add(txb);
        }
    }
}
//End~~~