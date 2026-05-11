using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;  // Linq and xml.Linq used to link my xml to this file
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Printing; // Email validation

namespace Contacts
{
   
    public partial class MainWindow: Window
    {
        List<Contact> contacts;
        public MainWindow()
        {
            InitializeComponent();
            XDocument doc = XDocument.Load("Contact.xml");
                contacts = doc.Descendants("Contact")
                .Select(c => new Contact
                {
                    Name = c.Element("Name")?.Value,
                    PhoneNumber = c.Element("PhoneNumber")?.Value,
                    Email = c.Element("Email")?.Value

                    
                })
                .ToList();

            Contact_List.ItemsSource = contacts;
        }

        public class Contact
        {
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
        }

        private void View_contact(object sender, RoutedEventArgs e)
        {
         Contact_List.Items.Refresh();
        }   // View Contact Function

        private void AddContacts(object sender, RoutedEventArgs e)
        {
            string name = TextName.Text;
            string phoneNumber = TextPhone.Text;
            string email = TextEmail.Text;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(email))  // if one or more boxes are empty - Error
            {
                MessageBox.Show($"Error!\n Please fill in the required fields");
                return;
            }
            else if(!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) // Checks if email has something before @. if it has an @. something after @ and if it has a . and something after .
            {
                MessageBox.Show("Error!\nPlease enter a valid email");
                return;
            }
            else if (phoneNumber.Length < 9) // If phone number is less than 9 digits - Error
            {
                MessageBox.Show($"Error!\n Phone number is too short");
                return;
            }
            else if(!phoneNumber.All(char.IsDigit)) // Phone number must be all digits - Error
            {
                MessageBox.Show($"Error!\n Phone number must be all digits");
                return;
            } else
            {
                contacts.Add(new Contact
                {
                    Name = name,
                    PhoneNumber = phoneNumber,
                    Email = email
                });
                Contact_List.Items.Refresh();
                MessageBox.Show("Contact added succesfully!");
            }

            
        } // Add contact function

        private void Save(object sender, RoutedEventArgs e) // Save function
        {
            var newXML = new XElement("Contacts",
                contacts.Select(c => new XElement("Contact",
                new XElement("Name", c.Name),
                new XElement("PhoneNumber", c.PhoneNumber),
                new XElement("Email", c.Email)
                ))
             );
            newXML.Save("Contact.xml"); // save file location

            MessageBox.Show("Contacts saved succesfully!");
            
        }

        private void Edit_Contact(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var contact = button.DataContext as Contact;

            TextName.Text = contact.Name;
            TextPhone.Text = contact.PhoneNumber;
            TextEmail.Text = contact.Email;
        }   // Edit button Function

        private void Remove_Contact(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var contact = button.DataContext as Contact;
            var result = MessageBox.Show(
                $"Are you sure you want to remove {contact.Name}?",
                    "Confirm delete",
                     MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
            );

            if( result == MessageBoxResult.Yes)
            {
                contacts.Remove(contact);
                Contact_List.Items.Refresh();
            }

          
        } // Remove button Function

        private void Window_closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show(
                "Do you want to save before exiting?",
                "Exit confirmation",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question
                );

            if (result == MessageBoxResult.Yes)
            {
                Save(null, null);
            }
            else if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }
    }

        
}