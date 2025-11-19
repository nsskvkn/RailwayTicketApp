using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RailwayTicketApp.Data;
using RailwayTicketApp.Models;

namespace RailwayTicketApp.Forms
{
    public partial class TrainManagementForm : Form
    {
        private RailwayDbContext dbContext;
        private DataGridView dataGridView; // Оголошено як поле класу

        public TrainManagementForm()
        {
            dbContext = new RailwayDbContext();
            InitializeComponent();
            LoadTrains();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1024, 600);
            this.Text = "Управління потягами";
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
            dataGridView = new DataGridView // Тепер поле класу
            {
                Name = "dataGridViewTrains",
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
            var btnView = CreateButton("Деталі", 3);
            var btnWagons = CreateButton("Вагони", 4);

            panelButtons.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnView, btnWagons });

            // Встановлюємо розташування кнопок
            int buttonWidth = 100;
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
                case 0: btn.Click += (s, e) => AddTrain(); break;
                case 1: btn.Click += (s, e) => EditTrain(dataGridView); break;
                case 2: btn.Click += (s, e) => DeleteTrain(dataGridView); break;
                case 3: btn.Click += (s, e) => ViewTrainDetails(dataGridView); break;
                case 4: btn.Click += (s, e) => ViewWagons(dataGridView); break;
            }

            return btn;
        }

        private void LoadTrains()
        {
            dataGridView.DataSource = dbContext.Trains.ToList(); // Використовуємо поле класу
        }

        private void AddTrain()
        {
            using (var form = new EditTrainForm(null, dbContext))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadTrains();
                }
            }
        }

        private void EditTrain(DataGridView dgv)
        {
            if (dgv.CurrentRow != null)
            {
                var trainId = (int)dgv.CurrentRow.Cells["TrainId"].Value;
                var train = dbContext.Trains.Find(trainId);
                if (train != null)
                {
                    using (var form = new EditTrainForm(train, dbContext))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadTrains();
                        }
                    }
                }
            }
        }

        private void DeleteTrain(DataGridView dgv)
        {
            if (dgv.CurrentRow != null)
            {
                var trainId = (int)dgv.CurrentRow.Cells["TrainId"].Value;
                var train = dbContext.Trains.Find(trainId);
                if (train != null)
                {
                    if (train.Bookings != null && train.Bookings.Any())
                    {
                        MessageBox.Show("Неможливо видалити потяг, який має бронювання.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var result = MessageBox.Show($"Видалити потяг {train.TrainNumber}?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        dbContext.Trains.Remove(train);
                        dbContext.SaveChanges();
                        LoadTrains();
                    }
                }
            }
        }

        private void ViewTrainDetails(DataGridView dgv)
        {
            if (dgv.CurrentRow != null)
            {
                var trainId = (int)dgv.CurrentRow.Cells["TrainId"].Value;
                var train = dbContext.Trains.Find(trainId);
                if (train != null)
                {
                    MessageBox.Show(
                        $"Назва: {train.TrainName}\n" +
                        $"Номер: {train.TrainNumber}\n" +
                        $"Маршрут: {train.DepartureStation} → {train.ArrivalStation}\n" +
                        $"Час відправлення: {train.DepartureTime:dd.MM.yyyy HH:mm}\n" +
                        $"Час прибуття: {train.ArrivalTime:dd.MM.yyyy HH:mm}\n" +
                        $"Ціна: {train.BasePrice:C2}",
                        "Деталі потяга",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private void ViewWagons(DataGridView dgv)
        {
            if (dgv.CurrentRow != null)
            {
                var trainId = (int)dgv.CurrentRow.Cells["TrainId"].Value;
                var train = dbContext.Trains.Find(trainId);
                if (train != null)
                {
                    string info = $"Потяг: {train.TrainNumber} – {train.TrainName}\n\n";
                    foreach (var w in train.Wagons)
                    {
                        double percentage = (double)w.BookedSeats / w.TotalSeats * 100;
                        info += $"🔹 {w.WagonType}: {w.BookedSeats}/{w.TotalSeats} ({percentage:F1}%)\n";
                    }
                    MessageBox.Show(info, "Вагони потяга", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
