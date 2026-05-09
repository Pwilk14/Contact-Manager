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
using System.Text.RegularExpressions; // Email validation

namespace Contacts
{
   
    public partial class MainWindow : Window
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
        }

        public class Contact
        {
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
        }




        private void View_contact(object sender, RoutedEventArgs e)
        {
            Contact_View.Text = "";

            foreach (var contact in contacts)
            {
                Contact_View.Text +=
                    $"Name: {contact.Name}\n" +
                    $"Phone number: {contact.PhoneNumber}\n" +
                    $"Email: {contact.Email}\n\n";
                   
            }
            

        }

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
                MessageBox.Show("Contact added succesfully!");
            }

            
        }

        private void Remove_Contact(object sender, RoutedEventArgs e)
        {
            string nameToRemove = TextName.Text;
            var contact2 = contacts.FirstOrDefault(c => c.Name.Equals(nameToRemove, StringComparison.OrdinalIgnoreCase));

            if (contact2 != null)  // if loop to remove contact
            {
                contacts.Remove(contact2);
                MessageBox.Show("Contact removed");
            }
            else
            {
                MessageBox.Show("Contact does not exist.\nPlease enter a valid contact");  // Validation if contact existsS
                return;
            }


        }

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
            this.Close();
        }
    }


}