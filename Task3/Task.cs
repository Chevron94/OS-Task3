using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Task3
{
    public struct proc_data
    {
        public uint proc_id;
        public string proc_name;
        public uint proc_memory;
    }
    
    class Task
    {
        DataGridView dgv;
        ToolStripStatusLabel status;
        public Task(ref DataGridView _dgv, ref ToolStripStatusLabel _status)
        {
            dgv = _dgv;
            status = _status;
        }

        public proc_data FindBiggestProcess()
        {
            List<ProcessEntry32> pr = Toolhelp32.GetAllProcess();
            List<HEAPENTRY32> tmp = new List<HEAPENTRY32>();
            proc_data pm = new proc_data();
            proc_data max = new proc_data();
            int k = 1;
            foreach (ProcessEntry32 t in pr)
            {
                status.Text = "Текущий процесс: " + t.szExeFile;
                pm = new proc_data(); 
                tmp = Toolhelp32.GetAllHeapsByProcess(t.th32ProcessID);
                if (tmp != null)
                {
                    dgv.Invoke(new Action(() => dgv.RowCount++));
                    pm.proc_id = t.th32ProcessID;
                    pm.proc_name = t.szExeFile;
                    foreach (HEAPENTRY32 h in tmp)
                        pm.proc_memory += h.dwBlockSize;
                    if (max.proc_memory < pm.proc_memory)
                    {
                        max = pm;
                        dgv.Invoke(new Action(() =>
                        {
                        dgv.Rows[0].Cells[0].Value = 0;
                        dgv.Rows[0].Cells[1].Value = pm.proc_id;
                        dgv.Rows[0].Cells[2].Value = t.szExeFile;
                        dgv.Rows[0].Cells[3].Value = pm.proc_memory;
                        }
                        ));
                    }
                    dgv.Invoke(new Action(() =>
                    {
                        dgv.Rows[k].Cells[0].Value = k;
                        dgv.Rows[k].Cells[1].Value = pm.proc_id;
                        dgv.Rows[k].Cells[2].Value = t.szExeFile;
                        dgv.Rows[k].Cells[3].Value = pm.proc_memory;
                    }
                   ));
                    k++;
                }
                else
                {
                    dgv.Invoke(new Action(() => dgv.RowCount++));
                    dgv.Invoke(new Action(() =>
                     {
                         dgv.Rows[k].Cells[0].Value = k;
                         dgv.Rows[k].Cells[1].Value = t.th32ProcessID;
                         dgv.Rows[k].Cells[2].Value = t.szExeFile;
                         dgv.Rows[k].Cells[3].Value = uint.MinValue;
                     }
                    ));
                    k++;
                }
                
            }
            status.Text = "Готово!";
            return max;
        }
    
    }
}
