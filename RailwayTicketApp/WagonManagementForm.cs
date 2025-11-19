using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RailwayTicketApp.Data;
using RailwayTicketApp.Models;

namespace RailwayTicketApp.Forms
{
    public partial class WagonManagementForm : Form
    {
        private RailwayDbContext dbContext;
        private DataGridView dataGridView; // ✅ Поле класу

        public WagonManagementForm()
        {
            dbContext = new RailwayDbContext();
            InitializeComponent();
            LoadWagons();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1024, 600);
            this.Text = "Управління вагонами";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // Головна панель
            var panelMain = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(panelMain);

            // DataGridView
            dataGridView = new DataGridView
            {
                Name = "dataGridViewWagons",
                Location = new Point(10, 10),
                Size = new Size(panelMain.ClientSize.Width - 20, panelMain.ClientSize.Height - 100),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Dock = DockStyle.Top,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(240, 245, 255) },
                ColumnHeadersDefaultCellStyle = {
                    BackColor = Color.FromArgb(0, 85, 170),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                },
                EnableHeadersVisualStyles = false
            };

            panelMain.Controls.Add(dataGridView);

            // Панель кнопок
            var panelButtons = new Panel
            {
                Location = new Point(10, dataGridView.Bottom + 10),
                Size = new Size(panelMain.ClientSize.Width - 20, 40),
                Dock = DockStyle.Bottom,
                BackColor = Color.Transparent
            };
            panelMain.Controls.Add(panelButtons);

            // Кнопки
            var btnAdd = CreateButton("Додати", 0);
            var btnEdit = CreateButton("Редагувати", 1);
            var btnDelete = CreateButton("Видалити", 2);
            var btnView = CreateButton("Переглянути місця", 3);

            panelButtons.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnView });

            // Встановлюємо розташування кнопок
            int buttonWidth = 120;
            int spacing = 10;
            for (int i = 0; i < panelButtons.Controls.Count; i++)
            {
                panelButtons.Controls[i].Location = new Point(i * (buttonWidth + spacing), 0);
                panelButtons.Controls[i].Size = new Size(buttonWidth, 30);
            }
        }

        private Button CreateButton(string text, int index)
        {
            var btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Tag = index
            };

            switch (index)
            {
                case 0: btn.Click += (s, e) => AddWagon(); break;
                case 1: btn.Click += (s, e) => EditWagon(dataGridView); break;
                case 2: btn.Click += (s, e) => DeleteWagon(dataGridView); break;
                case 3: btn.Click += (s, e) => ViewWagonSeats(dataGridView); break;
            }

            return btn;
        }

        private void LoadWagons()
        {
            dataGridView.DataSource = dbContext.Wagons.ToList();
        }

        private void AddWagon()
        {
            using (var form = new EditWagonForm(null, dbContext))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadWagons();
                }
            }
        }

        private void EditWagon(DataGridView dgv)
        {
            if (dgv.CurrentRow != null)
            {
                var wagonId = (int)dgv.CurrentRow.Cells["WagonId"].Value;
                var wagon = dbContext.Wagons.Find(wagonId);
                if (wagon != null)
                {
                    using (var form = new EditWagonForm(wagon, dbContext))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadWagons();
                        }
                    }
                }
            }
        }

        private void DeleteWagon(DataGridView dgv)
        {
            if (dgv.CurrentRow != null)
            {
                var wagonId = (int)dgv.CurrentRow.Cells["WagonId"].Value;
                var wagon = dbContext.Wagons.Find(wagonId);
                if (wagon != null)
                {
                    if (wagon.BookedSeats > 0)
                    {
                        MessageBox.Show("Вагон має заброньовані місця і не може бути видалений.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var result = MessageBox.Show($"Видалити вагон {wagon.WagonType}?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        dbContext.Wagons.Remove(wagon);
                        dbContext.SaveChanges();
                        LoadWagons();
                    }
                }
            }
        }

        private void ViewWagonSeats(DataGridView dgv)
        {
            if (dgv.CurrentRow != null)
            {
                var wagonId = (int)dgv.CurrentRow.Cells["WagonId"].Value;
                var wagon = dbContext.Wagons.Find(wagonId);
                if (wagon != null)
                {
                    string info = $"Вагон: {wagon.WagonType}\n" +
                                  $"Зайнято: {wagon.BookedSeats}/{wagon.TotalSeats}\n" +
                                  $"Вільно: {wagon.TotalSeats - wagon.BookedSeats}\n" +
                                  $"Зайнятість: {((double)wagon.BookedSeats / wagon.TotalSeats * 100):F1}%";
                    MessageBox.Show(info, "Стан місць", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}