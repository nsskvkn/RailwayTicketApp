using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RailwayTicketApp.Data;
using RailwayTicketApp.Models;

namespace RailwayTicketApp.Forms
{
    public partial class BookingManagementForm : Form
    {
        private RailwayDbContext dbContext;
        private DataGridView dataGridView; // Поле класу

        public BookingManagementForm()
        {
            dbContext = new RailwayDbContext();
            InitializeComponent();
            LoadBookings();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1024, 600);
            this.Text = "Управління бронюваннями";
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
                Name = "dataGridViewBookings",
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
            var btnCancel = CreateButton("Скасувати", 2);
            var btnView = CreateButton("Переглянути", 3);

            panelButtons.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnCancel, btnView });

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
                case 0: btn.Click += (s, e) => AddBooking(); break;
                case 1: btn.Click += (s, e) => EditBooking(dataGridView); break;
                case 2: btn.Click += (s, e) => CancelBooking(dataGridView); break;
                case 3: btn.Click += (s, e) => ViewBookingDetails(dataGridView); break;
            }

            return btn;
        }

        private void LoadBookings()
        {
            dataGridView.DataSource = dbContext.Bookings.ToList();
        }

        private void AddBooking()
        {
            using (var form = new EditBookingForm(null, dbContext))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadBookings();
                }
            }
        }

        private void EditBooking(DataGridView dgv)
        {
            if (dgv.CurrentRow != null)
            {
                var bookingId = (int)dgv.CurrentRow.Cells["BookingId"].Value;
                var booking = dbContext.Bookings.Find(bookingId);
                if (booking != null)
                {
                    using (var form = new EditBookingForm(booking, dbContext))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadBookings();
                        }
                    }
                }
            }
        }

        private void CancelBooking(DataGridView dgv)
        {
            if (dgv.CurrentRow != null)
            {
                var bookingId = (int)dgv.CurrentRow.Cells["BookingId"].Value;
                var booking = dbContext.Bookings.Find(bookingId);
                if (booking != null)
                {
                    if (booking.Status == "Скасовано")
                    {
                        MessageBox.Show("Бронювання вже скасоване.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var result = MessageBox.Show($"Скасувати бронювання для {booking.PassengerName}?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        booking.Status = "Скасовано";

                        // Звільняємо місце у вагоні
                        var wagon = dbContext.Wagons.Find(booking.WagonId);
                        if (wagon != null && wagon.BookedSeats > 0)
                        {
                            wagon.BookedSeats--;
                        }

                        dbContext.SaveChanges();
                        LoadBookings();
                    }
                }
            }
        }

        private void ViewBookingDetails(DataGridView dgv)
        {
            if (dgv.CurrentRow != null)
            {
                var bookingId = (int)dgv.CurrentRow.Cells["BookingId"].Value;
                var booking = dbContext.Bookings.Find(bookingId);
                if (booking != null)
                {
                    MessageBox.Show(
                        $"Пасажир: {booking.PassengerName}\n" +
                        $"Документ: {booking.PassengerDocument}\n" +
                        $"Поїзд: {booking.Train.TrainNumber} - {booking.Train.TrainName}\n" +
                        $"Вагон: {booking.Wagon.WagonType}\n" +
                        $"Місце: {booking.SeatNumber}\n" +
                        $"Статус: {booking.Status}\n" +
                        $"Дата бронювання: {booking.BookingDate:dd.MM.yyyy HH:mm}",
                        "Деталі бронювання",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }
    }
}
