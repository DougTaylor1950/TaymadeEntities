using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TaymadeEntities.Models;
using TaymadeEntities.ViewModels;
using DocumentFormat.OpenXml.Vml;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;

//using System.Windows.Forms;
using Button = Avalonia.Controls.Button;
using Avalonia.Platform;

namespace TaymadeEntities.Dialogs;

public partial class EditCastMemberDialog : Window
{
    private int pointerClickCount;

    public EditCastMemberDialog()
    {
        InitializeComponent();
        DataContextChanged += this.EditCastMemberDialog_DataContextChanged;

        //if (this.ButtonPanelMale != null)
        //{
        // this.ButtonPanelMale.SmallUpButtonClick(IncreaseMaleAge);
        // this.ButtonPanelMale.SmallDownButtonClick(DecreaseMaleAge);
        // this.ButtonPanelMale.SmallPlusButtonClick(AddMales);
        // this.ButtonPanelMale.SmallMinusButtonClick(RemoveMales);
        //}

        //if (this.ButtonPanelFemale != null)
        //{
        //    this.ButtonPanelFemale.SmallUpButtonClick(IncreaseFemaleAge);
        //    this.ButtonPanelFemale.SmallDownButtonClick(DecreaseFemaleAge);
        //    this.ButtonPanelFemale.SmallPlusButtonClick(AddFemales);
        //    this.ButtonPanelFemale.SmallMinusButtonClick(RemoveFemales);
        //}
    }

    private void RemoveFemales(object? sender, RoutedEventArgs e)
    {
        string? newText = this.cbText.Text;
        if (this.cbText.Text.Length > 0)
        {
            newText = this.cbText.Text.Substring(0, this.cbText.Text.Length - 1);
        }
        this.SetViewModelCastInfo(newText);
    }

    private void AddFemales(object? sender, RoutedEventArgs e)
    {
        string? newText = this.cbText.Text;
        if (this.cbText.Text.Length > 0)
        {
            newText = this.cbText.Text + this.cbText.Text[this.cbText.Text.Length - 1];
        }
        this.SetViewModelCastInfo(newText);
    }

    private void DecreaseFemaleAge(object? sender, RoutedEventArgs e)
    {
        string? newText = this.cbText.Text;
        if (this.cbText.Text.Length > 0)
        {
            int len = this.cbText.Text.Length;
            char ch = this.cbText.Text[len - 1];
            if (ch == 'f')
                ch = 'g';
            else ch = char.ToLower(ch);


            newText = this.cbText.Text.Substring(0, len - 1) + ch;
        }
        this.SetViewModelCastInfo(newText);
    }

    private void IncreaseFemaleAge(object? sender, RoutedEventArgs e)
    {
        string? newText = this.cbText.Text;
        if (this.cbText.Text.Length > 0)
        {
            int len = this.cbText.Text.Length;
            char ch = this.cbText.Text[len - 1];
            ch = char.ToUpper(ch);
            if (ch == 'G')
                ch = 'f';
            newText = this.cbText.Text.Substring(0, len - 1) + ch;
        }
        this.SetViewModelCastInfo(newText);
    }

    private void RemoveMales(object? sender, RoutedEventArgs e)
    {
        string? newText = this.cbText.Text;
        if (this.cbText.Text.Length > 0)
        {
            newText = this.cbText.Text.Substring(1);
        }
        this.SetViewModelCastInfo(newText);

    }

    private void AddMales(object? sender, RoutedEventArgs e)
    {
        string newText = StoryViewModel.CurrentCastPhrase;
        if (newText.Length > 0)
        {
            newText = newText[0] + newText;
            StoryViewModel.CurrentCastPhrase = newText;
        }
        this.SetViewModelCastInfo(newText);
    }

    private void DecreaseMaleAge(object? sender, RoutedEventArgs e)
    {
        string newText = this.cbText.Text;
        if (this.cbText.Text.Length > 0)
        {
            char ch = this.cbText.Text[0];
            if (ch == 'm')
            {
                ch = 'b';
            }
            else
                ch = char.ToLower(ch);
            newText = ch + this.cbText.Text.Substring(1);
        }
        this.SetViewModelCastInfo(newText);
    }
    private void IncreaseMaleAge(object? sender, RoutedEventArgs e)
    {
        string newText = StoryViewModel.CurrentCastPhrase;
        if (newText.Length > 0)
        {
            char ch = newText[0];
            if (ch == 'b') ch = 'm';
            else
                ch = char.ToUpper(ch);
            newText = ch + newText.Substring(1);
        }
        SetViewModelCastInfo(newText);
    }

    private void SetViewModelCastInfo(string newText)
    {
        StoryViewModel = this.DataContext as StoryViewModel;
        if (StoryViewModel != null) StoryViewModel.CurrentCastPhrase = newText;
        this.cbText.Text = newText;
    }

    //public StoryViewModel? StoryViewModel { get; private set; }
    public DateTime LastClick { get; private set; }

    private void EditCastMemberDialog_DataContextChanged(object? sender, System.EventArgs e)
    {
        if (this.DataContext != null)
        {
            StoryViewModel = this.DataContext as StoryViewModel;
        }
    }

    public void Relation_DoubleTapped(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // change the relationship in the CurrentCastPhrase

        CheckViewModel();

        if (StoryViewModel != null && !string.IsNullOrEmpty(StoryViewModel.CurrentCastPhrase))
        {
            string entry = StoryViewModel.CurrentCastPhrase;

            int indx = entry.IndexOf("inc:");
            if (indx >= 0)
            {
                indx += 4;

                string start = entry.Substring(0, indx);
                start += this.cbRelation.Text;
                StoryViewModel.CurrentCastPhrase = start;
            }
        }

    }

    private void CheckViewModel()
    {
        if (DataContext != null && StoryViewModel == null && DataContext is StoryViewModel)
        {
            StoryViewModel = this.DataContext as StoryViewModel;
        }
    }

    private void AgeButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CheckViewModel();

        if (StoryViewModel != null && StoryViewModel.CurrentCastMember != null)
        {
            if (string.IsNullOrEmpty(StoryViewModel.CurrentCastMember.Age))
            {
                StoryViewModel.CurrentCastMember.Age = StoryViewModel.CurrentAge.ToString().Trim();
                this.SetCodesAge();
            }
            else
            {
                StoryViewModel.CurrentCastMember.Age += "," + StoryViewModel.CurrentAge.ToString();
                this.SetCodesAge();
            }
        }
    }

    private void SetCodesAge()
    {
        if (string.IsNullOrEmpty(StoryViewModel.CurrentCastMember.Codes))
            StoryViewModel.CurrentCastMember.Codes += StoryViewModel.CurrentAge.ToString().Trim() + "y";
        else
        {
            StoryViewModel.CurrentCastMember.Codes += " - " + StoryViewModel.CurrentAge.ToString().Trim() + "y";
        }
    }

    private void CodesButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.SetCodes();

    }

    private void SetCodes()
    {
        CheckViewModel();

        if (StoryViewModel != null && StoryViewModel.CurrentCastMember != null)
        {
            if (string.IsNullOrEmpty(StoryViewModel.CurrentCastMember.Codes))
            {
                // just add code text
                StoryViewModel.CurrentCastMember.Codes += StoryViewModel.CurrentCastPhrase;
            }
            else StoryViewModel.CurrentCastMember.Codes += "," + StoryViewModel.CurrentCastPhrase;

        }
    }

    private void SpaceButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CheckViewModel();

        if (StoryViewModel != null && StoryViewModel.CurrentCastMember != null)
        {
            if (string.IsNullOrEmpty(StoryViewModel.CurrentCastMember.Codes))
            {
                // just add code text
                // StoryViewModel.CurrentCastMember.Codes += " ";
            }
            else StoryViewModel.CurrentCastMember.Codes += ", ";

        }
    }

    private void ComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (this != null && e.AddedItems != null && e.AddedItems.Count > 0)
        {
            this.cbText.Text = e.AddedItems[0].ToString();
            if (StoryViewModel == null)
            {
                if (this.DataContext != null && this.DataContext is StoryViewModel)
                {
                    StoryViewModel = this.DataContext as StoryViewModel;
                }
            }

            if (this.StoryViewModel != null) this.StoryViewModel.CurrentCastPhrase = this.cbText.Text;
        }
    }

    private void TextBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        this.SetCodes();
    }

    private void ComboBox_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (e.Pointer.Type is Avalonia.Input.PointerType.Mouse)
        {
            
            TimeSpan ts = DateTime.Now - LastClick;
            //Debug.WriteLine(ts.TotalMilliseconds);
            //TimeSpan dtTime = IPlatformSettings.GetDoubleTapTime(Avalonia.Input.PointerType.Mouse);
            //Debug.WriteLine(dtTime.TotalMilliseconds);
            if (1000 * 3 > ts.TotalMilliseconds)
            {
                // Code to handle double click goes here
                if (sender is Avalonia.Controls.ComboBox)
                {
                    Avalonia.Controls.ComboBox? cb = sender as Avalonia.Controls.ComboBox;
                    //
                    if (cb != null)
                    {
                        if (cb.Name == "cbText")
                        {
                            this.SetCodes();
                        }
                        else if (cb.Name == "ageCombo")
                        {
                            this.SetCodesAge();
                        }
                    }
                }
            }
            this.LastClick = DateTime.Now;
        }
    }

    private void ComboBox_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        pointerClickCount = e.ClickCount;
        Debug.WriteLine(pointerClickCount.ToString());
    }

    /// <summary>
    /// Defines the phrases.
    /// </summary>
    // private List<string>? phrases = new List<string>() { "mf", "mg", "Mf", "Mg", "Mg,inc:F", "bg", "inc:F", "inc:U", "inc:B", "fk", "fk:1st", "anal", "anal:1st", "fl", "ff", "cl", "dp", "gb", "dg", "hj", "nc" };





}