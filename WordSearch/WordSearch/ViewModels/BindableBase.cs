using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using WordSearch.Helpers;
using Xamarin.Forms;

namespace WordSearch.ViewModels
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        // Store navigation reference
        internal INavigation Navigation { get; set; }

        public BindableBase(INavigation navigation)
        {
            Navigation = navigation;
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            //TODO: when we remove the old OnPropertyChanged method we need to uncomment the below line
            //OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
#pragma warning disable CS0618 // Type or member is obsolete
            OnPropertyChanged(propertyName);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Obsolete("Please use the new RaisePropertyChanged method. This method will be removed to comply wth .NET coding standards. If you are overriding this method, you should overide the OnPropertyChanged(PropertyChangedEventArgs args) signature instead.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        // back to main page
        internal void CloseWindow()
        {
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopToRootAsync(true);
                });
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"CloseWindow exception, {ex.Message}");
            }
        }
    }
}