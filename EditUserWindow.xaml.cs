using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.DataAnnotations;
using library_management_system.Models;
using System.Linq;
using System.Collections.Generic;

namespace library_management_system
{
    public partial class EditUserWindow : Window
    {
        public User? UpdatedUser { get; private set; }
        private readonly User _originalUser;

        public EditUserWindow(User user)
        {
            InitializeComponent();
            _originalUser = user;

            // Initialize with current values
            txtName.Text = user.Name;
            txtEmail.Text = user.Email;

            // Setup role checkboxes
            switch (user.Role)
            {
                case "Admin":
                    chkAdmin.IsChecked = true;
                    break;
                case "Librarian":
                    chkLibrarian.IsChecked = true;
                    break;
                default: // Member
                    chkMember.IsChecked = true;
                    break;
            }

            // Setup status checkboxes
            switch (user.Status)
            {
                case "Inactive":
                    chkInactive.IsChecked = true;
                    break;
                case "Suspended":
                    chkSuspended.IsChecked = true;
                    break;
                default: // Active
                    chkActive.IsChecked = true;
                    break;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Determine selected role
            string selectedRole = chkAdmin.IsChecked == true ? "Admin" :
                                chkLibrarian.IsChecked == true ? "Librarian" : "Member";

            // Determine selected status
            string selectedStatus = chkActive.IsChecked == true ? "Active" :
                                  chkInactive.IsChecked == true ? "Inactive" : "Suspended";

            // Create a temporary user for vaalidation
            var tempUser = new User
            {
                Id = _originalUser.Id,
                Name = txtName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Role = selectedRole,
                Status = selectedStatus,
                CreatedAt = _originalUser.CreatedAt
            };

            // Validate the user with explicit namespace specification
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(tempUser);
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(tempUser, validationContext, validationResults, true);

            if (!isValid)
            {
                MessageBox.Show(string.Join("\n", validationResults.Select(r => r.ErrorMessage)),
                                "Validation Errors",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            // Update the original user's properties
            _originalUser.Name = tempUser.Name;
            _originalUser.Email = tempUser.Email;
            _originalUser.Role = tempUser.Role;
            _originalUser.Status = tempUser.Status;
            _originalUser.UpdatedAt = DateTime.UtcNow;

            UpdatedUser = _originalUser;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void RoleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var current = sender as CheckBox;
            if (current == null || current.IsChecked != true) return;

            foreach (var cb in new[] { chkAdmin, chkLibrarian, chkMember })
            {
                if (cb != current) cb.IsChecked = false;
            }
        }

        private void StatusCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var current = sender as CheckBox;
            if (current == null || current.IsChecked != true) return;

            foreach (var cb in new[] { chkActive, chkInactive, chkSuspended })
            {
                if (cb != current) cb.IsChecked = false;
            }
        }
    }
}