﻿using System.Diagnostics;
using System.Globalization;
using BebooGarden.GameCore;

namespace BebooGarden.Interface.EscapeMenu;

public class MainMenu : Form
{
  public Form? Result;
  public MainMenu(string title, Dictionary<string, Form> choices)
  {
    WindowState = FormWindowState.Maximized;
    Choices = choices;
    Label lblTitle = new();
    Text = IGlobalActions.GetLocalizedString(title);
    lblTitle.Text = IGlobalActions.GetLocalizedString(title);
    lblTitle.AutoSize = true;
    Controls.Add(lblTitle);

    for (int i = 0; i < Choices.Keys.Count; i++)
    {
      string choiceText = Choices.Keys.ElementAt(i);
      Button btnOption = new();
      btnOption.Text = choiceText;
      btnOption.Click += btn_Click;
      btnOption.Enter += btn_enter;
      btnOption.KeyDown += KeyHandle;
      Controls.Add(btnOption);
    }
    Button commandsButton = new()
    {
      Text = IGlobalActions.GetLocalizedString("ui.commands")
    };
    commandsButton.Click += OpenCommands;
    commandsButton.Enter += btn_enter;
    commandsButton.KeyDown += KeyHandle;
    Controls.Add(commandsButton);
    Button websiteButton = new()
    {
      Text = "website"
    };
    websiteButton.Click += OpenWebsite;
    websiteButton.Enter += btn_enter;
    websiteButton.KeyDown += KeyHandle;
    Controls.Add(websiteButton);
    Button discordButton = new()
    {
      Text = "Discord"
    };
    discordButton.Click += InviteDiscord;
    discordButton.Enter += btn_enter;
    discordButton.KeyDown += KeyHandle;
    Controls.Add(discordButton);
    Button quitButton = new()
    {
      Text = IGlobalActions.GetLocalizedString("ui.quit")
    };
    quitButton.Click += Quit;
    quitButton.Enter += btn_enter;
    quitButton.KeyDown += KeyHandle;
    Controls.Add(quitButton);
    Button back = new();
    back.Text = IGlobalActions.GetLocalizedString("ui.back");
    back.AccessibleDescription = Choices.Keys.Count + 1 + "/" + (Choices.Keys.Count + 1);
    back.Click += Back;
    back.Enter += btn_enter;
    back.KeyDown += KeyHandle;
    Controls.Add(back);
    Game.ResetKeyState();
  }

  private void Quit(object? sender, EventArgs e)
  {
    Game.GameWindow.Close();
    Back(sender, e);
  }

  private void OpenCommands(object? sender, EventArgs e)
  {
    var twoLetterLang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
    var langFile = Path.Combine(SoundSystem.CONTENTFOLDER, "doc", $"commands_{twoLetterLang}.md");
    var file = Path.Combine(SoundSystem.CONTENTFOLDER, "doc", "commands.md");
    if (File.Exists(langFile))
      Process.Start(new ProcessStartInfo(langFile) { UseShellExecute = true });
    else if (File.Exists(file))
      Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });
  }
  private void InviteDiscord(object? sender, EventArgs e)
  {
    Process.Start(new ProcessStartInfo(Secrets.DISCORDINVITE) { UseShellExecute = true });
  }

  protected void btn_Click(object sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuOkSound);
    Button clickedButton = (Button)sender;
    Result = Choices[clickedButton.Text];
    Result.ShowDialog(this);
    if (Result is Inventory || Result is Teleport) Close();
  }
  private void OpenWebsite(object? sender, EventArgs e)
  {
    Process.Start(new ProcessStartInfo("https://www.example.com") { UseShellExecute = true });
  }
  private void KeyHandle(object? sender, KeyEventArgs e)
  {
    switch (e.KeyCode)
    {
      case Keys.Escape:
      case Keys.Back:
        Back(sender, EventArgs.Empty);
        break;
      case Keys.F4:
        if (e.Modifiers == Keys.Alt) Back(sender, EventArgs.Empty);
        break;
    }
  }
  protected virtual void Back(object? sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuReturnSound);
    Result = default(Form);
    Close();
  }

  protected Dictionary<string, Form> Choices { get; }

  private void btn_enter(object? sender, EventArgs e)
  {
    Game.SoundSystem.System.PlaySound(Game.SoundSystem.MenuBipSound);
  }

}
