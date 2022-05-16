using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace P4GFusionEditor
{
    public partial class FusionEditor : Form
    {
        public static string[] PersonaNames = new string[] { };
        public int SelectedRow;
        public DataGridView SelectedGrid;
        public Bin fclTable;
        public FusionEditor()
        {
            ReadPersonaNames();

            InitializeComponent();

            LoadPersonaNames();
        }
        void LoadFusion(string Name,Fcl Normal)
        {
            DataGridView View = new DataGridView();
            var controls = GetAll(this, typeof(DataGridView));
            foreach (Control c in controls)
            {
                if (c.Name.EndsWith(Name))
                    View = c as DataGridView;
                
            }
            View.Rows.Clear();
            for (int i = 0; i < Normal.Entries.Count; i++)
            {
                int newRowIndex = View.Rows.Add();
                DataGridViewRow row = View.Rows[newRowIndex];
                row.Cells[0].Value = $"{newRowIndex + 1}";
                DataGridViewComboBoxCell Res = row.Cells[1] as DataGridViewComboBoxCell;
                Res.Value = Res.Items[Normal.Entries[i].ResultId];
                for (int j = 0; j < row.Cells.Count-2; j++)
                {
                    DataGridViewComboBoxCell Mat = row.Cells[j+2] as DataGridViewComboBoxCell;
                    Mat.Value = Mat.Items[Normal.Entries[i].MaterialIds[j]];
                }

            }
        }
        Fcl SaveFusion(string Name)
        {
            DataGridView View = new DataGridView();
            var controls = GetAll(this, typeof(DataGridView));
            foreach (Control c in controls)
            {
                if (c.Name.EndsWith(Name))
                    View = c as DataGridView;

            }
            Fcl output = new Fcl();
            for (int i = 0; i < View.Rows.Count; i++)
            {
                FclEntry Entry = new FclEntry();
                DataGridViewComboBoxCell Res = View.Rows[i].Cells[1] as DataGridViewComboBoxCell;
                Entry.ResultId = (ushort)Res.Items.IndexOf(Res.Value.ToString());

                for (int j = 0; j < View.Rows[i].Cells.Count - 2; j++)
                {
                    DataGridViewComboBoxCell Mat = View.Rows[i].Cells[j + 2] as DataGridViewComboBoxCell;

                    Entry.MaterialIds.Add((ushort)Mat.Items.IndexOf(Mat.Value.ToString()));
                }

                output.Entries.Add(Entry);
            }

            return output;
        }
        IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }
        public static void ReadPersonaNames()
        {
            string TxtPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\PersonaNames.txt";
            if (File.Exists(TxtPath))
            {
                PersonaNames = File.ReadAllLines(TxtPath);
            }
            else
            {
                MessageBox.Show("PersonaNames.txt was not found\nPlease download and place it in the program's directory");
                System.Windows.Forms.Application.Exit();
            }
        }
        void LoadPersonaNames()
        {
            List<string> InputNames = new List<string>();
            
            for (int i = 0; i < PersonaNames.Length; i++)
            {
                if (true) // TODO: add this as an option in options
                    InputNames.Add($"{i} {PersonaNames[i]}");
                else
                    InputNames.Add($"{PersonaNames[i]}");

            }
            var controls = GetAll(this, typeof(DataGridView));
            foreach (Control c in controls)
            {
                DataGridView pain = c as DataGridView;
                foreach (DataGridViewColumn col in pain.Columns)
                {
                    if (col.HeaderText != "#")
                    {
                        DataGridViewComboBoxColumn temp = col as DataGridViewComboBoxColumn;
                        temp.Items.Clear();
                        temp.Items.AddRange(InputNames.ToArray());
                    }
                }
            }
        }


        void LoadFusionTables(Bin InputBin)
        {
            fclTable = InputBin;

            int index = InputBin.FindBinIndex("fclCombineTable_NormalSP.ftd");
            Fcl Normal = new Fcl(InputBin.Files[index].Data);
            index = InputBin.FindBinIndex("fclCombineTable_ThirdSP.ftd");
            Fcl Triangle = new Fcl(InputBin.Files[index].Data);
            index = InputBin.FindBinIndex("fclCombineTable_Fourth.ftd");
            Fcl Cross = new Fcl(InputBin.Files[index].Data);
            index = InputBin.FindBinIndex("fclCombineTable_Fifth.ftd");
            Fcl Pentagon = new Fcl(InputBin.Files[index].Data);
            index = InputBin.FindBinIndex("fclCombineTable_Sixth.ftd");
            Fcl Hexagon = new Fcl(InputBin.Files[index].Data);
            index = InputBin.FindBinIndex("fclCombineTable_Twelfth.ftd");
            Fcl Dodecagon = new Fcl(InputBin.Files[index].Data);
            LoadFusion("Normal",Normal);
            LoadFusion("Triangle", Triangle);
            LoadFusion("Cross", Cross);
            LoadFusion("Pentagon", Pentagon);
            LoadFusion("Hexagon", Hexagon);
            LoadFusion("Dodecagon", Dodecagon);

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileToolStripMenuItem.HideDropDown();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "fclTable.bin (*.bin)|*.bin";
            dialog.Title = "Load your fclTable.bin...";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Bin InputBin = new Bin(dialog.FileName);
                if (InputBin.Files[0].Name != "fclWeaponTypeName.ftd")
                {
                    int index = InputBin.FindBinIndex("fclTable.bin");
                    if (index == -1)
                    {
                        MessageBox.Show("not a proper fclTable.bin");
                        return;
                    }
                    else
                    {
                        Bin TempBin = new Bin(InputBin.Files[index].Data);
                        InputBin = TempBin;
                    }
                }

                LoadFusionTables(InputBin);
            }

            
                
        }


        private void dataGridViewNormal_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FusionEditor_Load(object sender, EventArgs e)
        {
            
            

        }

        private void dataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    DataGridViewCell clickedCell = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex];

                    clickedCell.DataGridView.CurrentCell = clickedCell;  // Select the clicked cell, for instance



                    var relativeMousePosition = clickedCell.DataGridView.PointToClient(Cursor.Position);

                    this.RowMenuStrip.Show(clickedCell.DataGridView, relativeMousePosition);

                    SelectedRow = e.RowIndex;
                    SelectedGrid = clickedCell.DataGridView;
                }
                
            }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddRow(SelectedGrid);
        }
        void AddRow(DataGridView GridView)
        {
            int index = GridView.Rows.Add();
            GridView.Rows[index].Cells[0].Value = $"{GridView.Rows.Count}";
            for (int i = 1; i < GridView.Rows[index].Cells.Count; i++)
            {
                DataGridViewComboBoxCell Cell = GridView.Rows[index].Cells[i] as DataGridViewComboBoxCell;
                Cell.Value = Cell.Items[0];
            }

            FixNumbers(GridView);
        }
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedGrid.Rows.Count != 0)
                SelectedGrid.Rows.RemoveAt(SelectedRow);

            FixNumbers(SelectedGrid);
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveRow(-1);
        }

        public void MoveRow(int Offset)
        {
            if (SelectedGrid.SelectedRows == null || SelectedGrid.SelectedRows.Count == 0)
                return;

            int index = SelectedGrid.SelectedRows[0].Index + Offset;

            if (index < 0 || index >= SelectedGrid.Rows.Count)
                return;

            DataGridViewRow SelectedRow = SelectedGrid.SelectedRows[0];

            SelectedGrid.Rows.Remove(SelectedRow);
            SelectedGrid.Rows.Insert(index, SelectedRow);

            SelectedGrid.Rows[index].Selected = true;

            FixNumbers(SelectedGrid);
        }

        public void FixNumbers(DataGridView InputGrid)
        {
            for (int i = 0; i < InputGrid.Rows.Count; i++)
                InputGrid.Rows[i].Cells[0].Value = $"{i + 1}";
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveRow(1);
        }

        private void dataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridView View = sender as DataGridView;
            if (View.Rows.Count == 0)
                AddRow(View);

        }

        private void aemulusFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileToolStripMenuItem.HideDropDown();
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "fclTable";
            dialog.Filter = "Folder | ";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SaveFolder(dialog.FileName);
            }
        }
        byte[] GetFclArray(string Name)
        {
            Fcl Normal = SaveFusion(Name);

            MemoryStream NormalStream = new MemoryStream();
            BinaryWriter NormalWriter = new BinaryWriter(NormalStream);
            Normal.Save(NormalWriter);
            return NormalStream.ToArray();
        }
        void SaveFolder(string FileName)
        {
            if (!Directory.Exists(FileName))
                Directory.CreateDirectory(FileName);
            byte[] Normal = GetFclArray("Normal");
            byte[] Triangle = GetFclArray("Triangle");
            byte[] Cross = GetFclArray("Cross");
            byte[] Pentagon = GetFclArray("Pentagon");
            byte[] Hexagon = GetFclArray("Hexagon");
            byte[] Dodecagon = GetFclArray("Dodecagon");
            
            File.WriteAllBytes(FileName + @"\fclCombineTable_NormalSP.ftd", Normal);
            File.WriteAllBytes(FileName + @"\fclCombineTable_ThirdSP.ftd", Triangle);
            File.WriteAllBytes(FileName + @"\fclCombineTable_Fourth.ftd", Cross);
            File.WriteAllBytes(FileName + @"\fclCombineTable_Fifth.ftd", Pentagon);
            File.WriteAllBytes(FileName + @"\fclCombineTable_Sixth.ftd", Hexagon);
            File.WriteAllBytes(FileName + @"\fclCombineTable_Twelfth.ftd", Dodecagon);

        }

        private void fclTablebinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileToolStripMenuItem.HideDropDown();

            if (fclTable == null)
            {
                MessageBox.Show("fclTable.bin not loaded, please load it");
                

                OpenFileDialog dialog2 = new OpenFileDialog();
                dialog2.Filter = "fclTable.bin or fclTable.bin (*.bin)|*.bin";
                dialog2.Title = "Load your fclTable.bin...";
                if (dialog2.ShowDialog() == DialogResult.OK)
                {
                    Bin InputBin = new Bin(dialog2.FileName);
                    if (InputBin.Files[0].Name != "fclWeaponTypeName.ftd")
                    {
                        int index = InputBin.FindBinIndex("fclTable.bin");
                        if (index == -1)
                        {
                            MessageBox.Show("fclTable.bin not found");
                            return;
                        }
                        else
                        {
                            Bin TempBin = new Bin(InputBin.Files[index].Data);
                            InputBin = TempBin;
                        }
                    }

                    fclTable = InputBin;

                }
                else
                    return;
            }


            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save your fclTable.bin...";
            dialog.FileName = "fclTable.bin";
            dialog.Filter = "fclTable.bin |*.bin";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                byte[] Normal = GetFclArray("Normal");
                byte[] Triangle = GetFclArray("Triangle");
                byte[] Cross = GetFclArray("Cross");
                byte[] Pentagon = GetFclArray("Pentagon");
                byte[] Hexagon = GetFclArray("Hexagon");
                byte[] Dodecagon = GetFclArray("Dodecagon");

                Bin NewBin = new Bin();
                for (int i =0; i < fclTable.Files.Count;i++)
                    NewBin.Files.Add(fclTable.Files[i]);


                int index = NewBin.FindBinIndex("fclCombineTable_NormalSP.ftd");
                NewBin.Files[index].Data = Normal;
                index = NewBin.FindBinIndex("fclCombineTable_ThirdSP.ftd");
                NewBin.Files[index].Data = Triangle;
                index = NewBin.FindBinIndex("fclCombineTable_Fourth.ftd");
                NewBin.Files[index].Data = Cross;
                index = NewBin.FindBinIndex("fclCombineTable_Fifth.ftd");
                NewBin.Files[index].Data = Pentagon;
                index = NewBin.FindBinIndex("fclCombineTable_Sixth.ftd");
                NewBin.Files[index].Data = Hexagon;
                index = NewBin.FindBinIndex("fclCombineTable_Twelfth.ftd");
                NewBin.Files[index].Data = Dodecagon;

                NewBin.Save(dialog.FileName);
            }
        }

        private void openSingleFtdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileToolStripMenuItem.HideDropDown();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "p4g Fusion Table (*.ftd)|*.ftd";
            dialog.Title = "Load your ftd...";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string Name = "Normal";
                Fcl InputFcl = new Fcl(dialog.FileName);

                int index = InputFcl.Entries[0].MaterialIds.IndexOf((ushort)0);

                switch (index)
                {
                    case 3: Name = "Triangle"; tabControl1.SelectedIndex = 1; break;
                    case 4: Name = "Cross"; tabControl1.SelectedIndex = 2;  break;
                    case 5: Name = "Pentagon"; tabControl1.SelectedIndex = 3; break;
                    case 6: Name = "Hexagon"; tabControl1.SelectedIndex = 4; break;
                    case -1: Name = "Dodecagon"; tabControl1.SelectedIndex = 5; break;
                    default: tabControl1.SelectedIndex = 0; break;
                }

                LoadFusion(Name, InputFcl);
            }
        }

        private void currentsTabFtdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileToolStripMenuItem.HideDropDown();
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save your ftd...";
            dialog.Filter = "P4G FusionTable |*.ftd";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                byte[] Output;
                switch (tabControl1.SelectedIndex)
                {
                    case 1: Output = GetFclArray("Triangle"); break;
                    case 2: Output = GetFclArray("Cross"); break;
                    case 3: Output = GetFclArray("Pentagon"); break;
                    case 4: Output = GetFclArray("Dodecagon"); break;
                    default: Output = GetFclArray("Normal"); break;
                }

                File.WriteAllBytes(dialog.FileName, Output);

            }


            
        }
    }
}
