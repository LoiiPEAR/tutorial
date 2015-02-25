using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace mobileservice
{
    public sealed partial class MainPage
    {
        string checkGender;
        bool checkStatus = false;
        profile pp;
        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.MobileService != null)
            {
                statusTbl.Text = "Connected";
                checkStatus = true;
            }
            else
            {
                statusTbl.Text = "Bye";
                checkStatus = false;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (checkStatus == true)
            {

                var test = await App.MobileService.GetTable<profile>().Where(m => m.Name == "p" && m.Surname == "p").ToCollectionAsync();
                if (test.Count > 0)
                {
                    string a = "";
                }

                if(maleRB.IsChecked==true){
                    checkGender = "Male";
                }
                else if(femaleRB.IsChecked==true){
                    checkGender = "Female";
                }
                profile myData = new profile()
                {
                    Name = nameTb.Text,
                    Surname = surnameTb.Text,
                    Email = mailTb.Text,
                    Telephone = telTb.Text,
                    Gender = checkGender
                };
                try
                {

                    await App.MobileService.GetTable<profile>().InsertAsync(myData);

                    MessageDialog msg = new MessageDialog("Save Complete");
                    msg.ShowAsync();

                    showData();
                }
                catch (MobileServiceInvalidOperationException ex)
                {
                    MessageDialog msg = new MessageDialog("Fail");
                    msg.ShowAsync();
                }
            }
        }

        public async void showData()
        {
            var getData = await App.MobileService.GetTable<profile>().ToCollectionAsync();
            myListView.ItemsSource = getData;
        }

        private void myListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pp = (sender as ListView).SelectedItem as profile;

            MessageDialog ms = new MessageDialog("Do you want to delete?");

            UICommand yesBtn = new UICommand("Yes");
            yesBtn.Invoked = yesBtnClick;
            ms.Commands.Add(yesBtn);
            UICommand noBtn = new UICommand("No");
            noBtn.Invoked = noBtnClick;
            ms.Commands.Add(noBtn);

            ms.ShowAsync();
        }

        private void noBtnClick(IUICommand command)
        {
            
        }

        private async void yesBtnClick(IUICommand command)
        {
            await App.MobileService.GetTable<profile>().DeleteAsync(pp);
            showData();
        }
    }
}
