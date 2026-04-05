using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic; 
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using static Aray.BaseArea;
using Button = System.Windows.Forms.Button;

namespace Aray
{
    public partial class BaseArea : Form
    {
        private Button selectedSlotButton = null;
        private Label selectedSlotLabel = null;
        public Dictionary<string, (Button btn, Label lbl)> slotMap;
        private BaseArea baseAreaForm;
        private UpperGround upperGroundForm;
        public BaseArea()
        {
            InitializeComponent();

            slotMap = new Dictionary<string, (Button, Label)>(StringComparer.OrdinalIgnoreCase)
            {
                { "Area 1", (btnA1, lblA1) },
                { "Area 2", (btnA2, lblA2) },
                { "Area 3", (btnA3, lblA3) },
                { "Area 4", (btnA4, lblA4) },
                { "Area 5", (btnA5, lblA5) },
                { "Area 6", (btnA6, lblA6) },
                { "Area 7", (btnA7, lblA7) },
                { "Area 8", (btnA8, lblA8) },
                { "Area 9", (btnA9, lblA9) },
                { "Area 10", (btnA10, lblA10) },
                { "Area 11", (btnA11, lblA11) },
                { "Area 12", (btnA12, lblA12) },
                { "Area 13", (btnA13, lblA13) },
                { "Area 14", (btnA14, lblA14) },
                { "Area 15", (btnA15, lblA15) }
             };

            // Assign the Tag property for each button
            btnA1.Tag = "Area 1";
            btnA2.Tag = "Area 2";
            btnA3.Tag = "Area 3";
            btnA4.Tag = "Area 4";
            btnA5.Tag = "Area 5";
            btnA6.Tag = "Area 6";
            btnA7.Tag = "Area 7";
            btnA8.Tag = "Area 8";
            btnA9.Tag = "Area 9";
            btnA10.Tag = "Area 10";
            btnA11.Tag = "Area 11";
            btnA12.Tag = "Area 12";
            btnA13.Tag = "Area 13";
            btnA14.Tag = "Area 14";
            btnA15.Tag = "Area 15";

            foreach (var kvp in slotMap)
                kvp.Value.btn.Tag = kvp.Key;

        }

        public int GetSlotCount()
        {
            return slotMap.Count; 
        }
        private void UpdateBaseAreaAvailability()
        {
            int occupiedSlots = 0;
            foreach (var slot in slotMap.Keys)
            {
                using (SqlConnection conn = new SqlConnection(@"Server=Unique;Database=ParkDB;Trusted_Connection=True;"))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM ParkingEntry WHERE SlotID=@slotID AND LeavingTime IS NULL";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@slotID", slot);
                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0) occupiedSlots++;
                    }
                }
            }

            lblBAA.Text = $"Base Area Availability: {slotMap.Count - occupiedSlots}/{slotMap.Count}";
        }


        public int GetOccupiedSlots()
        {


            int occupied = 0;
            foreach (var slot in slotMap.Keys)
            {
                using (SqlConnection conn = new SqlConnection(@"Server=Unique;Database=ParkDB;Trusted_Connection=True;"))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM ParkingEntry WHERE SlotID=@slotID AND LeavingTime IS NULL";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@slotID", slot);
                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0) occupied++;
                    }
                }
            }
            return occupied;
        }
        public int GetUpperGroundOccupied()
        {
            if (upperGroundForm == null) return 0;
            return upperGroundForm.GetOccupiedSlots();
        }

        public void SetUpperGroundForm(UpperGround ugForm)
        {
            upperGroundForm = ugForm;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        public string AutoClassifyVehicle(string model)
        {
            model = model.Trim().ToLower();

            // MOTORCYCLE
            string[] motorcycles = { "raider", "click", "aerox", "nmax", "sniper", "rouser", "mio", "r15", "r3" };
            if (motorcycles.Any(m => model.Contains(m)))
                return "Motorcycle";

            // LARGE VEHICLES
            string[] largeVehicles =
            {
        "fortuner", "montero", "everest", "patrol", "land cruiser",
        "hilux", "navara", "strada", "ranger", "raptor", "wildtrak",
        "hiace", "starex", "urvan", "innova", "expander",
        "suv", "pickup", "van"
    };

            if (largeVehicles.Any(m => model.Contains(m)))
                return "Large";

            // SMALL VEHICLES
            string[] smallVehicles =
            {
        "vios", "city", "civic", "altis", "corolla", "yaris",
        "mirage", "wigo", "brio", "mazda 2", "mazda2",
        "swift", "picanto", "accent"
    };

            if (smallVehicles.Any(m => model.Contains(m)))
                return "Small";

            // DEFAULT
            return "Small";  // fallback to small
        }


        private void btnEnter_Click(object sender, EventArgs e)
        {
            string inputSlot = Microsoft.VisualBasic.Interaction.InputBox(
      "Enter the slot number (Example: Area 1, Area 2, ...):",
      "Slot Entry",
      ""
  );

            if (string.IsNullOrWhiteSpace(inputSlot))
            {
                MessageBox.Show("Slot is required!");
                return;
            }

            inputSlot = inputSlot.Trim();

            if (!slotMap.ContainsKey(inputSlot))
            {
                MessageBox.Show("Slot not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the button and label for the typed slot
            var (slotButton, slotLabel) = slotMap[inputSlot];

            string connectionString = @"Server=Unique;Database=ParkDB;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if slot is occupied
                string checkQuery = "SELECT COUNT(*) FROM ParkingEntry WHERE SlotID = @slotID AND LeavingTime IS NULL";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@slotID", inputSlot);
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0)
                    {
                        MessageBox.Show("This slot is already occupied!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Ask for car model
                string carModel = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter the Car Model (Example: Raptor, Vios, Hilux, Aerox):",
                    "Car Model",
                    ""
                );

                if (string.IsNullOrWhiteSpace(carModel))
                {
                    MessageBox.Show("Car model is required.");
                    return;
                }

                string vehicleType = AutoClassifyVehicle(carModel);

                // Insert the parking entry
                string insertQuery = "INSERT INTO ParkingEntry (SlotID, VehicleType, EntryTime) VALUES (@slotID, @vehicleType, @entryTime)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@slotID", inputSlot);
                    cmd.Parameters.AddWithValue("@vehicleType", vehicleType);
                    cmd.Parameters.AddWithValue("@entryTime", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }

            // Update UI
            slotButton.BackColor = Color.Red;
            slotLabel.Text = DateTime.Now.ToString("HH:mm");

            // Update availability
            UpdateBaseAreaAvailability();

            MessageBox.Show(
                "Please take note of the slot number where your vehicle is parked.",
                "Parking Reminder",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        private void btnA1_Click(object sender, EventArgs e)
        {
            if (selectedSlotButton != null)
            {
                selectedSlotButton.BackColor = Color.Green;
            }

            selectedSlotButton = sender as Button;
            selectedSlotLabel = lblA1;

            btnA1.BackColor = Color.Yellow;
        }

        private void BaseArea_Load(object sender, EventArgs e)
        {

        }
        private void btnBA_Click(object sender, EventArgs e)
        {
            if (upperGroundForm == null)
            {
                MessageBox.Show("UpperGround form is not initialized!");
                return;
            }
            this.Hide();
            upperGroundForm.RefreshSlots(); // optional
            upperGroundForm.Show();
        }
        public void RefreshSlots()
        {
            foreach (var slot in slotMap)
            {
                string slotID = slot.Key;
                var (btn, lbl) = slot.Value;

                using (SqlConnection conn = new SqlConnection(@"Server=Unique;Database=ParkDB;Trusted_Connection=True;"))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM ParkingEntry WHERE SlotID=@slotID AND LeavingTime IS NULL";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@slotID", slotID);
                        int count = (int)cmd.ExecuteScalar();

                        btn.BackColor = count > 0 ? Color.Red : Color.Green;
                        lbl.Text = count > 0 ? lbl.Text : "";
                    }
                }
            }

            if (upperGroundForm != null)
                lblUGA.Text = $"Upper Ground Occupied: {GetUpperGroundOccupied()}/{upperGroundForm.GetSlotCount()}";

            // Update BaseArea availability
            UpdateBaseAreaAvailability();  // This updates lblUGA
        }

        private void btnLeave_Click(object sender, EventArgs e)
        {
            string inputSlot = Microsoft.VisualBasic.Interaction.InputBox(
                  "Enter the slot number to free:",
                  "Leaving Slot",
                  ""
               );

            if (string.IsNullOrWhiteSpace(inputSlot))
            {
                MessageBox.Show("No slot entered!");
                return;
            }

            if (!slotMap.ContainsKey(inputSlot))
            {
                MessageBox.Show("Slot not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            (selectedSlotButton, selectedSlotLabel) = slotMap[inputSlot];

            // Update button and label visually
            selectedSlotButton.BackColor = Color.Green;
            selectedSlotLabel.Text = "";

            DateTime leavingTime = DateTime.Now;
            leavingTime = leavingTime.AddMilliseconds(-leavingTime.Millisecond);

            string connectionString = @"Server=Unique;Database=ParkDB;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Get entry info
                string getQuery = "SELECT EntryTime, VehicleType FROM ParkingEntry " +
                                  "WHERE SlotID = @slotID AND LeavingTime IS NULL";

                DateTime entryTime;
                string vehicleType = "";

                using (SqlCommand cmd = new SqlCommand(getQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@slotID", inputSlot);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show("No active parking record found.");
                            return;
                        }

                        entryTime = (DateTime)reader["EntryTime"];
                        vehicleType = reader["VehicleType"].ToString();
                    }
                }

                // Update leaving time
                string updateQuery = "UPDATE ParkingEntry " +
                                     "SET LeavingTime = @leavingTime " +
                                     "WHERE SlotID = @slotID AND LeavingTime IS NULL";

                using (SqlCommand cmdUpdate = new SqlCommand(updateQuery, conn))
                {
                    cmdUpdate.Parameters.AddWithValue("@slotID", inputSlot);
                    cmdUpdate.Parameters.AddWithValue("@leavingTime", leavingTime);
                    cmdUpdate.ExecuteNonQuery();

                    UpdateBaseAreaAvailability();
                }

                // Calculate parking duration and fee
                TimeSpan duration = leavingTime - entryTime;
                double hours = duration.TotalHours;
                int roundedHours = (int)Math.Ceiling(hours);

                int rate = 0;

                switch (vehicleType)
                {
                    case "Large":
                        rate = 100;
                        break;
                    case "Small":
                        rate = 70;
                        break;
                    case "Motorcycle":
                        rate = 40;
                        break;
                    default:
                        rate = 70; // fallback for unknown vehicle types
                        break;
                }

                double totalFee = rate * roundedHours;

                // Automatically apply 30% surcharge if parked more than 1 hour
                if (hours > 1)
                    totalFee *= 1.3;

                MessageBox.Show(
                    $"Parking Receipt\n" +
                    $"------------------------------\n" +
                    $"Slot: {inputSlot}\n" +
                    $"Vehicle: {vehicleType}\n" +
                    $"Entry Time: {entryTime}\n" +
                    $"Leaving Time: {leavingTime}\n" +
                    $"Hours Parked: {roundedHours}\n" +
                    $"Rate per Hour: {rate}\n" +
                    $"Total Fee: ₱{totalFee:0.00}",
                    "Payment Summary",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Reset selection
                selectedSlotButton = null;
                selectedSlotLabel = null;
            }
        }

        private void btnLT_Click(object sender, EventArgs e)
        {
            int lostTicketFee = 300;

            string inputSlot = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the slot number for the lost ticket:",
                "Lost Ticket",
                ""
            );

            if (string.IsNullOrWhiteSpace(inputSlot))
            {
                MessageBox.Show("No slot entered!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!slotMap.ContainsKey(inputSlot))
            {
                MessageBox.Show("Slot not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            (selectedSlotButton, selectedSlotLabel) = slotMap[inputSlot];

            string connectionString = @"Server=Unique;Database=ParkDB;Trusted_Connection=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if slot is currently occupied
                string checkQuery = "SELECT COUNT(*) FROM ParkingEntry WHERE SlotID = @slotID AND LeavingTime IS NULL";
                using (SqlCommand cmdCheck = new SqlCommand(checkQuery, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@slotID", inputSlot);
                    int count = (int)cmdCheck.ExecuteScalar();

                    if (count == 0)
                    {
                        MessageBox.Show("This slot is not currently occupied. Cannot process lost ticket.",
                                        "Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        return;
                    }
                }

                // Update leaving time
                string updateQuery = "UPDATE ParkingEntry SET LeavingTime = @leavingTime WHERE SlotID = @slotID AND LeavingTime IS NULL";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@slotID", inputSlot);
                    cmd.Parameters.AddWithValue("@leavingTime", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }

            // Update UI
            selectedSlotButton.BackColor = Color.Green;
            selectedSlotLabel.Text = "";

            // Update availability
            UpdateBaseAreaAvailability();

            // Show receipt
            MessageBox.Show(
                $"Lost Ticket Receipt\n" +
                $"------------------------------\n" +
                $"Slot: {inputSlot}\n" +
                $"Lost Ticket Fee: ₱{lostTicketFee:0.00}",
                "Payment Summary",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            // Reset selection
            selectedSlotButton = null;
            selectedSlotLabel = null;
        }

        private void btnUG_Click(object sender, EventArgs e)
        {
            if (upperGroundForm == null)
            {
                MessageBox.Show("UpperGround form is not initialized!");
                return;
            }

            upperGroundForm.RefreshSlots();

            this.Hide();
            upperGroundForm.Show();
        }

        private void btnActArea_Click(object sender, EventArgs e)
        {
            string url = "https://parkinganph.com/parking/park-solutions-automated-multi-level-parking-pasig-city-98f3";
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot open the website: " + ex.Message);
            }
        }
    }
}

