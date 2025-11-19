using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RailwayTicketApp.Data;
using RailwayTicketApp.Models;

namespace RailwayTicketApp.Forms
{
    public partial class EditBookingForm : Form
    {
        private Booking booking;
        private RailwayDbContext dbContext;

        public EditBookingForm(Booking existingBooking, RailwayDbContext context)
        {
            booking = existingBooking;
            dbContext = context;
            InitializeComponent();
            LoadData();
            if (booking != null)
            {
                PopulateFields();
            }
        }

        private void InitializeComponent()
        {
            this.Size = new Size(460, 500);
            this.Text = booking == null ? "Додати бронювання" : "Редагувати бронювання";
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(245, 245, 245);

            // Заголовок
            var lblTitle = new Label
            {
                Text = this.Text,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 51, 102),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 40,
                BackColor = Color.FromArgb(230, 230, 230)
            };
            this.Controls.Add(lblTitle);

            // Панель форми
            var panelForm = new Panel
            {
                Location = new Point(10, 50),
                Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(panelForm);

            // Елементи форми
            var labels = new[]
            {
                "Пасажир:", "Документ:", "Поїзд:", "Вагон:", "Місце:", "Статус:"
            };

            int labelWidth = 90;
            int inputWidth = 280;
            int rowHeight = 45;
            int topMargin = 15;

            for (int i = 0; i < labels.Length; i++)
            {
                var lbl = new Label
                {
                    Text = labels[i],
                    Location = new Point(20, topMargin + i * rowHeight),
                    Size = new Size(labelWidth, 20),
                    ForeColor = Color.FromArgb(50, 50, 50)
                };
                panelForm.Controls.Add(lbl);

                if (i == 0) // Пасажир
                {
                    var txt = new TextBox
                    {
                        Name = "txtPassenger",
                        Location = new Point(120, topMargin + i * rowHeight),
                        Size = new Size(inputWidth, 20),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    panelForm.Controls.Add(txt);
                }
                else if (i == 1) // Документ
                {
                    var txt = new TextBox
                    {
                        Name = "txtDocument",
                        Location = new Point(120, topMargin + i * rowHeight),
                        Size = new Size(inputWidth, 20),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    panelForm.Controls.Add(txt);
                }
                else if (i == 2) // Поїзд
                {
                    var cmb = new ComboBox
                    {
                        Name = "cmbTrain",
                        Location = new Point(120, topMargin + i * rowHeight),
                        Size = new Size(inputWidth, 20),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    cmb.SelectedIndexChanged += (s, e) => OnTrainChanged(cmb, panelForm); // ✅ Додано обробник
                    panelForm.Controls.Add(cmb);
                }
                else if (i == 3) // Вагон
                {
                    var cmb = new ComboBox
                    {
                        Name = "cmbWagon",
                        Location = new Point(120, topMargin + i * rowHeight),
                        Size = new Size(inputWidth, 20),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    cmb.SelectedIndexChanged += (s, e) => OnWagonChanged(cmb, panelForm); // ✅ Додано обробник
                    panelForm.Controls.Add(cmb);
                }
                else if (i == 4) // Місце
                {
                    var cmb = new ComboBox
                    {
                        Name = "cmbSeat",
                        Location = new Point(120, topMargin + i * rowHeight),
                        Size = new Size(inputWidth, 20),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    panelForm.Controls.Add(cmb);
                }
                else if (i == 5) // Статус
                {
                    var cmb = new ComboBox
                    {
                        Name = "cmbStatus",
                        Location = new Point(120, topMargin + i * rowHeight),
                        Size = new Size(inputWidth, 20),
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    cmb.Items.AddRange(new[] { "Заброньовано", "Продано", "Скасовано" });
                    panelForm.Controls.Add(cmb);
                }
            }

            // Кнопки — в окремій панелі знизу
            var panelButtons = new Panel
            {
                Location = new Point(10, panelForm.Bottom + 10),
                Size = new Size(this.ClientSize.Width - 20, 40),
                Dock = DockStyle.Bottom,
                BackColor = Color.Transparent
            };
            this.Controls.Add(panelButtons);

            var btnOK = new Button
            {
                Text = "Зберегти",
                Location = new Point(100, 5),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnOK.Click += (s, e) => SaveBooking(
                panelForm.Controls["txtPassenger"] as TextBox,
                panelForm.Controls["txtDocument"] as TextBox,
                panelForm.Controls["cmbTrain"] as ComboBox,
                panelForm.Controls["cmbWagon"] as ComboBox,
                panelForm.Controls["cmbSeat"] as ComboBox,
                panelForm.Controls["cmbStatus"] as ComboBox);

            var btnCancel = new Button
            {
                Text = "Скасувати",
                Location = new Point(220, 5),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(150, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            panelButtons.Controls.AddRange(new Control[] { btnOK, btnCancel });
        }

        private void LoadData()
        {
            var cmbTrain = this.Controls.Find("cmbTrain", true)[0] as ComboBox;
            var trains = dbContext.Trains.ToList();
            foreach (var t in trains)
            {
                cmbTrain.Items.Add(new { Text = $"{t.TrainNumber} - {t.TrainName}", Value = t.TrainId });
            }
        }

        private void PopulateFields()
        {
            var txtPassenger = this.Controls.Find("txtPassenger", true)[0] as TextBox;
            var txtDocument = this.Controls.Find("txtDocument", true)[0] as TextBox;
            var cmbTrain = this.Controls.Find("cmbTrain", true)[0] as ComboBox;
            var cmbWagon = this.Controls.Find("cmbWagon", true)[0] as ComboBox;
            var cmbSeat = this.Controls.Find("cmbSeat", true)[0] as ComboBox;
            var cmbStatus = this.Controls.Find("cmbStatus", true)[0] as ComboBox;

            txtPassenger.Text = booking.PassengerName;
            txtDocument.Text = booking.PassengerDocument;
            var trains = dbContext.Trains.ToList();
            var selectedTrain = trains.FirstOrDefault(t => t.TrainId == booking.TrainId);
            if (selectedTrain != null)
            {
                cmbTrain.SelectedItem = new { Text = $"{selectedTrain.TrainNumber} - {selectedTrain.TrainName}", Value = selectedTrain.TrainId };
                LoadWagonsForTrain(selectedTrain.TrainId, cmbWagon);
                cmbWagon.SelectedValue = booking.WagonId;
                LoadSeatsForWagon(booking.WagonId, cmbSeat);
                cmbSeat.SelectedItem = booking.SeatNumber;
            }
            cmbStatus.SelectedItem = booking.Status;
        }

        private void OnTrainChanged(ComboBox cmbTrain, Panel panelForm)
        {
            var cmbWagon = panelForm.Controls.Find("cmbWagon", true)[0] as ComboBox;
            var cmbSeat = panelForm.Controls.Find("cmbSeat", true)[0] as ComboBox;

            cmbWagon.Items.Clear();
            cmbSeat.Items.Clear();

            if (cmbTrain.SelectedItem != null)
            {
                var selectedTrain = cmbTrain.SelectedItem as dynamic;
                LoadWagonsForTrain(selectedTrain.Value, cmbWagon);
            }
        }

        private void OnWagonChanged(ComboBox cmbWagon, Panel panelForm)
        {
            var cmbSeat = panelForm.Controls.Find("cmbSeat", true)[0] as ComboBox;

            cmbSeat.Items.Clear();

            if (cmbWagon.SelectedItem != null)
            {
                var selectedWagon = cmbWagon.SelectedItem as dynamic;
                LoadSeatsForWagon(selectedWagon.Value, cmbSeat);
            }
        }

        private void LoadWagonsForTrain(int trainId, ComboBox cmbWagon)
        {
            cmbWagon.Items.Clear();
            var wagons = dbContext.Wagons.Where(w => w.TrainId == trainId).ToList();
            foreach (var w in wagons)
            {
                cmbWagon.Items.Add(new { Text = $"{w.WagonType} ({w.TotalSeats - w.BookedSeats} вільних)", Value = w.WagonId });
            }
        }

        private void LoadSeatsForWagon(int wagonId, ComboBox cmbSeat)
        {
            cmbSeat.Items.Clear();
            var wagon = dbContext.Wagons.Find(wagonId);
            if (wagon != null)
            {
                int currentBookingId = booking?.BookingId ?? 0;
                for (int i = 1; i <= wagon.TotalSeats; i++)
                {
                    var existingBooking = dbContext.Bookings.FirstOrDefault(b => b.WagonId == wagonId && b.SeatNumber == i && b.BookingId != currentBookingId);
                    if (existingBooking == null)
                    {
                        cmbSeat.Items.Add(i);
                    }
                    else
                    {
                        if (booking != null && booking.SeatNumber == i)
                        {
                            cmbSeat.Items.Add(i);
                        }
                    }
                }
            }
        }

        private void SaveBooking(TextBox txtPassenger, TextBox txtDocument, ComboBox cmbTrain, ComboBox cmbWagon, ComboBox cmbSeat, ComboBox cmbStatus)
        {
            if (string.IsNullOrWhiteSpace(txtPassenger.Text) || string.IsNullOrWhiteSpace(txtDocument.Text) ||
                cmbTrain.SelectedItem == null || cmbWagon.SelectedItem == null || cmbSeat.SelectedItem == null || cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Заповніть всі поля!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (booking == null)
            {
                booking = new Booking();
                dbContext.Bookings.Add(booking);
            }

            booking.PassengerName = txtPassenger.Text.Trim();
            booking.PassengerDocument = txtDocument.Text.Trim();
            var selectedTrain = cmbTrain.SelectedItem as dynamic;
            booking.TrainId = selectedTrain.Value;
            var selectedWagon = cmbWagon.SelectedItem as dynamic;
            booking.WagonId = selectedWagon.Value;
            booking.SeatNumber = (int)cmbSeat.SelectedItem;
            booking.Status = cmbStatus.SelectedItem.ToString();

            // Якщо статус "Продано", то збільшуємо BookedSeats у вагоні
            if (booking.Status == "Продано")
            {
                var wagon = dbContext.Wagons.Find(booking.WagonId);
                if (wagon != null)
                {
                    if (booking.BookingId == 0 || booking.Status != "Продано")
                    {
                        wagon.BookedSeats++;
                    }
                }
            }

            // Якщо статус змінюється на "Скасовано", то зменшуємо BookedSeats
            else if (booking.Status == "Скасовано")
            {
                var wagon = dbContext.Wagons.Find(booking.WagonId);
                if (wagon != null && wagon.BookedSeats > 0)
                {
                    wagon.BookedSeats--;
                }
            }

            try
            {
                dbContext.SaveChanges();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}