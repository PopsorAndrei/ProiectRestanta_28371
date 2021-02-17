using GameLotModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Restanta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        GameLotEntitiesModel ctx = new GameLotEntitiesModel();
        CollectionViewSource customerViewSource;

        Binding txtFirstnameBinding = new Binding();
        Binding txtLastnameBinding = new Binding();
        Binding txtPhoneBinding = new Binding();


        CollectionViewSource productViewSource;
        Binding txtColorBinding = new Binding();
        Binding txtCPUBinding = new Binding();
        Binding txtGPUBinding = new Binding();
        Binding txtMakerBinding = new Binding();
        Binding txtPriceBinding = new Binding();

        CollectionViewSource customerOrdersViewSource;
        Binding txtCustomer = new Binding();
        Binding txtProduct = new Binding();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;


            txtPhoneBinding.Path = new PropertyPath("Phone");
            txtFirstnameBinding.Path = new PropertyPath("Firstname");
            txtLastnameBinding.Path = new PropertyPath("Lastname");

            txtColorBinding.Path = new PropertyPath("Color");
            txtCPUBinding.Path = new PropertyPath("CPU");
            txtGPUBinding.Path = new PropertyPath("GPU");
            txtMakerBinding.Path = new PropertyPath("Maker");
            txtPriceBinding.Path = new PropertyPath("Price");



            phoneTextBox.SetBinding(TextBox.TextProperty, txtPhoneBinding);
            firstnameTextBox.SetBinding(TextBox.TextProperty, txtFirstnameBinding);
            lastnameTextBox.SetBinding(TextBox.TextProperty, txtLastnameBinding);

            colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
            cPUTextBox.SetBinding(TextBox.TextProperty, txtCPUBinding);
            gPUTextBox.SetBinding(TextBox.TextProperty, txtGPUBinding);
            makerTextBox.SetBinding(TextBox.TextProperty, txtMakerBinding);
            priceTextBox.SetBinding(TextBox.TextProperty, txtPriceBinding);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            customerViewSource =((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            ctx.Customers.Load();
          
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            customerOrdersViewSource.Source = ctx.Orders.Local;

            ctx.Customers.Load();

            ctx.Orders.Load();
            ctx.Products.Load();

            cmbCustomers.ItemsSource = ctx.Customers.Local;
            cmbCustomers.DisplayMemberPath = "Firstname";
            cmbCustomers.SelectedValuePath = "CustId";

            cmbProducts.ItemsSource = ctx.Products.Local;
            cmbProducts.DisplayMemberPath = "Color";
            cmbProducts.SelectedValuePath = "CarId";

            productViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productViewSource")));
            productViewSource.Source = ctx.Products.Local;
            ctx.Products.Load();

            BindDataGrid();
        }
        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustomerId equals
                             cust.CustomerId
                             join prod in ctx.Products on ord.PcId equals
                             prod.PcId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CustomerId,
                                 ord.PcId,

                                 cust.Firstname,
                                 cust.Lastname,          
                                 cust.Phone,

                                 prod.Color,
                                 prod.CPU,
                                 prod.GPU,
                                 prod.Maker,
                                 prod.Price,
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {

                    customer = new Customer()
                    {
      
                        Firstname = firstnameTextBox.Text.Trim(),
                        Lastname = lastnameTextBox.Text.Trim(),
                        Phone = phoneTextBox.Text.Trim()
                    };

                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();

                    ctx.SaveChanges();
                }

                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;

                firstnameTextBox.IsEnabled = true;
                lastnameTextBox.IsEnabled = true;
                phoneTextBox.IsEnabled = true;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.Firstname = firstnameTextBox.Text.Trim();
                    customer.Lastname = lastnameTextBox.Text.Trim();
                    customer.Phone = phoneTextBox.Text.Trim();

                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();

                customerViewSource.View.MoveCurrentTo(customer);

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;

                firstnameTextBox.IsEnabled = false;
                lastnameTextBox.IsEnabled = false;

                phoneTextBox.IsEnabled = false;

            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                customerDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;

                firstnameTextBox.IsEnabled = false;
                lastnameTextBox.IsEnabled = false;
                phoneTextBox.IsEnabled = false;

                firstnameTextBox.SetBinding(TextBox.TextProperty, txtFirstnameBinding);
                lastnameTextBox.SetBinding(TextBox.TextProperty, txtLastnameBinding);
                phoneTextBox.SetBinding(TextBox.TextProperty, txtPhoneBinding);
            }
            SetValidationBinding();
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;

            firstnameTextBox.IsEnabled = true;
            lastnameTextBox.IsEnabled = true;
            phoneTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstnameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastnameTextBox, TextBox.TextProperty);
            firstnameTextBox.Text = "";
            lastnameTextBox.Text = "";
            phoneTextBox.Text = "";
            Keyboard.Focus(firstnameTextBox);
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempFirstName = firstnameTextBox.Text.ToString();
            string tempLastName = lastnameTextBox.Text.ToString();

            string tempPhone = phoneTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;

            firstnameTextBox.IsEnabled = true;
            lastnameTextBox.IsEnabled = true;
            phoneTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstnameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastnameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(phoneTextBox, TextBox.TextProperty);

            firstnameTextBox.Text = tempFirstName;
            lastnameTextBox.Text = tempLastName;

            phoneTextBox.Text = tempPhone;

            Keyboard.Focus(firstnameTextBox);
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            customerDataGrid.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;

            firstnameTextBox.IsEnabled = false;
            lastnameTextBox.IsEnabled = false;
            phoneTextBox.IsEnabled = false;

            firstnameTextBox.SetBinding(TextBox.TextProperty, txtFirstnameBinding);
            lastnameTextBox.SetBinding(TextBox.TextProperty, txtLastnameBinding);

            phoneTextBox.SetBinding(TextBox.TextProperty, txtPhoneBinding);

        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempFirstName = firstnameTextBox.Text.ToString();
            string tempLastName = lastnameTextBox.Text.ToString();
            string tempPhone = phoneTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;

            BindingOperations.ClearBinding(firstnameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastnameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(phoneTextBox, TextBox.TextProperty);

            firstnameTextBox.Text = tempFirstName;
            lastnameTextBox.Text = tempLastName;
            phoneTextBox.Text = tempPhone;
        }
        private void custPrev_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void custNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        //buttons for products
        private void prodSave_Click(object sender, RoutedEventArgs e)
        {
            Product product = null;
            if (action == ActionState.New)
            {
                try
                {

                    product = new Product()
                    {
                        Color = colorTextBox.Text.Trim(),
                        CPU  = cPUTextBox.Text.Trim(),
                        GPU = gPUTextBox.Text.Trim(),
                        Maker = makerTextBox.Text.Trim(),
                        Price = priceTextBox.Text.Trim(),
                    };


                    ctx.Products.Add(product);
                    productViewSource.View.Refresh();

                    ctx.SaveChanges();
                }

                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                prodNew.IsEnabled = true;
                prodEdit.IsEnabled = true;
                prodSave.IsEnabled = false;
                prodCancel.IsEnabled = false;
                productDataGrid.IsEnabled = true;
                prodPrev.IsEnabled = true;
                prodNext.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                cPUTextBox.IsEnabled = false;
                gPUTextBox.IsEnabled = false;
                makerTextBox.IsEnabled = false;
                priceTextBox.IsEnabled = false;

            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    product = (Product)productDataGrid.SelectedItem;
                    product.Color = colorTextBox.Text.Trim();
                    product.CPU = cPUTextBox.Text.Trim();
                    product.GPU = gPUTextBox.Text.Trim();
                    product.Maker= makerTextBox.Text.Trim();
                    product.Price = priceTextBox.Text.Trim();


                    ctx.SaveChanges();

                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                productViewSource.View.Refresh();

                productViewSource.View.MoveCurrentTo(product);
                prodNew.IsEnabled = true;
                prodEdit.IsEnabled = true;
                prodDelete.IsEnabled = true;

                prodSave.IsEnabled = false;
                prodCancel.IsEnabled = false;
                productDataGrid.IsEnabled = true;
                prodPrev.IsEnabled = true;
                prodNext.IsEnabled = true;

                colorTextBox.IsEnabled = false;
                cPUTextBox.IsEnabled = false;
                gPUTextBox.IsEnabled = false;
                makerTextBox.IsEnabled = false;
                priceTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    product = (Product)productDataGrid.SelectedItem;
                    ctx.Products.Remove(product);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                productViewSource.View.Refresh();

                prodNew.IsEnabled = true;
                prodEdit.IsEnabled = true;
                prodDelete.IsEnabled = true;

                prodSave.IsEnabled = false;
                prodCancel.IsEnabled = false;
                productDataGrid.IsEnabled = true;
                prodPrev.IsEnabled = true;
                prodNext.IsEnabled = true;

                colorTextBox.IsEnabled = false;
                cPUTextBox.IsEnabled = false;
                gPUTextBox.IsEnabled = false;
                makerTextBox.IsEnabled = false;
                priceTextBox.IsEnabled = false;

                colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
                cPUTextBox.SetBinding(TextBox.TextProperty, txtCPUBinding);
                gPUTextBox.SetBinding(TextBox.TextProperty, txtGPUBinding);
                makerTextBox.SetBinding(TextBox.TextProperty, txtMakerBinding);
                priceTextBox.SetBinding(TextBox.TextProperty, txtPriceBinding);

            }

            SetValidationBinding();
        }
        private void prodNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            prodNew.IsEnabled = false;
            prodEdit.IsEnabled = false;
            prodDelete.IsEnabled = false;

            prodSave.IsEnabled = true;
            prodCancel.IsEnabled = true;
            productDataGrid.IsEnabled = false;
            prodPrev.IsEnabled = false;
            prodNext.IsEnabled = false;

            colorTextBox.IsEnabled = true;
            cPUTextBox.IsEnabled = true;
            gPUTextBox.IsEnabled = true;
            makerTextBox.IsEnabled = true;
            priceTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(cPUTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(gPUTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makerTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(priceTextBox, TextBox.TextProperty);
            colorTextBox.Text = "";
            cPUTextBox.Text = "";
            gPUTextBox.Text = "";
            makerTextBox.Text = "";
            priceTextBox.Text = "";
            Keyboard.Focus(colorTextBox);
        }
        private void prodEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempColor = colorTextBox.Text.ToString();
            string tempCpu = cPUTextBox.Text.ToString();
            string tempGpu = gPUTextBox.Text.ToString();
            string tempMaker = makerTextBox.Text.ToString();
            string tempPrice = priceTextBox.Text.ToString();

            prodNew.IsEnabled = false;
            prodEdit.IsEnabled = false;
            prodDelete.IsEnabled = false;

            prodSave.IsEnabled = true;
            prodCancel.IsEnabled = true;
            productDataGrid.IsEnabled = false;
            prodPrev.IsEnabled = false;
            prodNext.IsEnabled = false;

            colorTextBox.IsEnabled = true;
            cPUTextBox.IsEnabled = true;
            gPUTextBox.IsEnabled = true;
            makerTextBox.IsEnabled = true;
            priceTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(cPUTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(gPUTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makerTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(priceTextBox, TextBox.TextProperty);

            colorTextBox.Text = tempColor;
            cPUTextBox.Text = tempCpu;
            gPUTextBox.Text = tempGpu;
            makerTextBox.Text = tempMaker;
            priceTextBox.Text = tempPrice;
            Keyboard.Focus(colorTextBox);
        }
        private void prodCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            prodNew.IsEnabled = true;
            prodEdit.IsEnabled = true;


            prodSave.IsEnabled = false;
            prodCancel.IsEnabled = false;
            productDataGrid.IsEnabled = true;
            prodPrev.IsEnabled = true;
            prodNext.IsEnabled = true;


            colorTextBox.IsEnabled = false;
            cPUTextBox.IsEnabled = false;
            gPUTextBox.IsEnabled = false;
            makerTextBox.IsEnabled = false;
            priceTextBox.IsEnabled = false;

            colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
            cPUTextBox.SetBinding(TextBox.TextProperty, txtCPUBinding);
            gPUTextBox.SetBinding(TextBox.TextProperty, txtGPUBinding);
            makerTextBox.SetBinding(TextBox.TextProperty, txtMakerBinding);
            priceTextBox.SetBinding(TextBox.TextProperty, txtPriceBinding);
        }
        private void prodDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempColor = colorTextBox.Text.ToString();
            string tempCpu = cPUTextBox.Text.ToString();
            string tempGpu = gPUTextBox.Text.ToString();
            string tempMaker = makerTextBox.Text.ToString();
            string tempPrice = priceTextBox.Text.ToString();

            prodNew.IsEnabled = false;
            prodEdit.IsEnabled = false;
            prodDelete.IsEnabled = false;

            prodSave.IsEnabled = true;
            prodCancel.IsEnabled = true;
            productDataGrid.IsEnabled = false;
            prodPrev.IsEnabled = false;
            prodNext.IsEnabled = false;


            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(cPUTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(gPUTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makerTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(priceTextBox, TextBox.TextProperty);

         

            colorTextBox.Text = tempColor;
            cPUTextBox.Text = tempCpu;
            gPUTextBox.Text = tempGpu;
            makerTextBox.Text = tempMaker;
            priceTextBox.Text = tempPrice;

        }
        private void prodPrev_Click(object sender, RoutedEventArgs e)
        {
            productViewSource.View.MoveCurrentToPrevious();
        }


        private void prodNext_Click(object sender, RoutedEventArgs e)
        {
            productViewSource.View.MoveCurrentToNext();
        }
        //butoane pentru orders
        private void ordNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            ordNew.IsEnabled = false;
            ordEdit.IsEnabled = false;
            ordDelete.IsEnabled = false;

            ordSave.IsEnabled = true;
            ordCancel.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            ordPrev.IsEnabled = false;
            ordNext.IsEnabled = false;

            cmbCustomers.IsEnabled = true;
            cmbProducts.IsEnabled = true;

            BindingOperations.ClearBinding(cmbCustomers, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbProducts, ComboBox.TextProperty);
            cmbCustomers.Text = "";
            cmbProducts.Text = "";
            Keyboard.Focus(cmbCustomers);
        }

        private void ordEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempCustomer = cmbCustomers.Text.ToString();
            string tempProduct = cmbProducts.Text.ToString();

            ordNew.IsEnabled = false;
            ordEdit.IsEnabled = false;
            ordDelete.IsEnabled = false;

            ordSave.IsEnabled = true;
            ordCancel.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            ordPrev.IsEnabled = false;
            ordNext.IsEnabled = false;

            cmbCustomers.IsEnabled = true;
            cmbProducts.IsEnabled = true;

            BindingOperations.ClearBinding(cmbCustomers, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbProducts, ComboBox.TextProperty);
            cmbCustomers.Text = tempCustomer;
            cmbProducts.Text = tempProduct;
            Keyboard.Focus(cmbCustomers);
        }

        private void ordDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempCustomer = cmbCustomers.Text.ToString();
            string tempProducts = cmbProducts.Text.ToString();

            ordNew.IsEnabled = false;
            ordEdit.IsEnabled = false;
            ordDelete.IsEnabled = false;

            ordSave.IsEnabled = true;
            ordCancel.IsEnabled = true;
            ordersDataGrid.IsEnabled = false;
            ordPrev.IsEnabled = false;
            ordNext.IsEnabled = false;


            BindingOperations.ClearBinding(cmbCustomers, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbProducts, ComboBox.TextProperty);
            cmbCustomers.Text = tempCustomer;
            cmbProducts.Text = tempProducts;
        }

        private void ordSave_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Product product = (Product)cmbProducts.SelectedItem;

                    //instantiem Order entity
                    order = new Order()
                    {
                        CustomerId = customer.CustomerId,
                        PcId = product.PcId,

                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();

                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                ordNew.IsEnabled = true;
                ordEdit.IsEnabled = true;
                ordSave.IsEnabled = false;
                ordCancel.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                ordPrev.IsEnabled = true;
                ordNext.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbProducts.IsEnabled = false;

            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;

                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustomerId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.PcId = Int32.Parse(cmbProducts.SelectedValue.ToString());
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                customerViewSource.View.Refresh();
                customerOrdersViewSource.View.MoveCurrentTo(order);
                ordNew.IsEnabled = true;
                ordEdit.IsEnabled = true;
                ordDelete.IsEnabled = true;

                ordSave.IsEnabled = false;
                ordCancel.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                ordPrev.IsEnabled = true;
                ordNext.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbProducts.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;

                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerOrdersViewSource.View.Refresh();
                ordNew.IsEnabled = true;
                ordEdit.IsEnabled = true;
                ordDelete.IsEnabled = true;

                ordSave.IsEnabled = false;
                ordCancel.IsEnabled = false;
                ordersDataGrid.IsEnabled = true;
                ordPrev.IsEnabled = true;
                ordNext.IsEnabled = true;
                cmbCustomers.IsEnabled = false;
                cmbProducts.IsEnabled = false;

                cmbCustomers.SetBinding(ComboBox.TextProperty, txtCustomer);
                cmbProducts.SetBinding(ComboBox.TextProperty, txtProduct);
            }

        }
        private void ordCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            ordNew.IsEnabled = true;
            ordEdit.IsEnabled = true;
            ordSave.IsEnabled = false;
            ordCancel.IsEnabled = false;
            ordersDataGrid.IsEnabled = true;
            ordPrev.IsEnabled = true;
            ordNext.IsEnabled = true;

            cmbCustomers.IsEnabled = false;
            cmbProducts.IsEnabled = false;

            cmbCustomers.SetBinding(ComboBox.TextProperty, txtCustomer);
            cmbProducts.SetBinding(ComboBox.TextProperty, txtProduct);
        }

        private void ordPrev_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void ordNext_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }









        private void SetValidationBinding()
        {
            Binding first_NameValidationBinding = new Binding();
            first_NameValidationBinding.Source = customerViewSource;
            first_NameValidationBinding.Path = new PropertyPath("FirstName");
            first_NameValidationBinding.NotifyOnValidationError = true;
            first_NameValidationBinding.Mode = BindingMode.TwoWay;
            first_NameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
           // first_NameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstnameTextBox.SetBinding(TextBox.TextProperty, first_NameValidationBinding);


            Binding last_NameValidationBinding = new Binding();
            last_NameValidationBinding.Source = customerViewSource;
            last_NameValidationBinding.Path = new PropertyPath("LastName");
            last_NameValidationBinding.NotifyOnValidationError = true;
            last_NameValidationBinding.Mode = BindingMode.TwoWay;
            last_NameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
          //  last_NameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastnameTextBox.SetBinding(TextBox.TextProperty, last_NameValidationBinding);




        }


    }
}

