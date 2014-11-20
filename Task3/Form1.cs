using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Task3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Thread = new Thread(new ThreadStart(MyThread));
            //OutputInfo.RowCount = 1;
            //OutputInfo.Rows[0].DefaultCellStyle.BackColor = Color.Yellow;
        }
        Thread Thread;
        void MyThread()
        {
            Task task = new Task(ref OutputInfo, ref StatusBar);
            proc_data tmp = task.FindBiggestProcess();
            MessageBox.Show("ID: " + tmp.proc_id + '\n' + "Владелец: " + tmp.proc_name + '\n' +"Размер памяти: " + tmp.proc_memory, "Владелец самой большого объема фиксированной памяти");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Thread.IsAlive)
                Thread.Abort();
        }

        private void Start_Click(object sender, EventArgs e)
        {
            OutputInfo.Rows.Clear();
            OutputInfo.RowCount = 1;
            OutputInfo.Rows[0].DefaultCellStyle.BackColor = Color.Yellow;
            Thread.Start();
        }
    }
}
