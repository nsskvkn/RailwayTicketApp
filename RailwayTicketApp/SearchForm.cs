using System;
using System.Linq;
using System.Windows.Forms;
using RailwayTicketApp.Data;
using RailwayTicketApp.Models;

namespace RailwayTicketApp.Forms
{
    public partial class SearchForm : Form  // ✅ Успадковується від Form
    {
        private RailwayDbContext dbContext;

        public SearchForm()
        {
            dbContext = new RailwayDbContext();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(800, 600);
            this.Text = "Пошук";
            this.StartPosition = FormStartPosition.CenterParent;

            var label = new Label();
            label.Text = "Ключове слово:";
            label.Location = new System.Drawing.Point(10, 10);
            this.Controls.Add(label);

            var textBox = new TextBox();
            textBox.Name = "txtKeyword";
            textBox.Location = new System.Drawing.Point(100, 10);
            textBox.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(textBox);

            var btnSearchTrains = new Button();
            btnSearchTrains.Text = "Пошук по потягах";
            btnSearchTrains.Location = new System.Drawing.Point(10, 40);
            btnSearchTrains.Click += (s, e) => SearchTrains(textBox.Text);
            this.Controls.Add(btnSearchTrains);

            var btnSearchBookings = new Button();
            btnSearchBookings.Text = "Пошук по даті бронювання";
            btnSearchBookings.Location = new System.Drawing.Point(150, 40);
            btnSearchBookings.Click += (s, e) => SearchBookingsByDate(textBox.Text);
            this.Controls.Add(btnSearchBookings);

            var dataGridView = new DataGridView();
            dataGridView.Location = new System.Drawing.Point(10, 80);
            dataGridView.Size = new System.Drawing.Size(760, 400);
            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToAddRows = false;
            this.Controls.Add(dataGridView);

            dataGridView.Name = "dataGridViewResults";
        }

        private void SearchTrains(string keyword)
        {
            var dgv = this.Controls.Find("dataGridViewResults", true).FirstOrDefault() as DataGridView;
            if (dgv != null)
            {
                var results = dbContext.Trains.Where(t =>
                    t.TrainNumber.Contains(keyword) ||
                    t.TrainName.Contains(keyword) ||
                    t.DepartureStation.Contains(keyword) ||
                    t.ArrivalStation.Contains(keyword)
                ).ToList();

                dgv.DataSource = results;
            }
        }

        private void SearchBookingsByDate(string dateStr)
        {
            var dgv = this.Controls.Find("dataGridViewResults", true).FirstOrDefault() as DataGridView;
            if (DateTime.TryParse(dateStr, out DateTime date))
            {
                var results = dbContext.Bookings.Where(b => b.BookingDate.Date == date.Date).ToList();
                dgv.DataSource = results;
            }
        }
    }
}