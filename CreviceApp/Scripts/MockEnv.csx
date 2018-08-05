﻿#r "..\\crevice4.exe"

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Crevice.Core.FSM;
using Crevice.Config;
using Crevice.UI;
using Crevice.UserScript;
using Crevice.UserScript.Keys;
using Crevice.GestureMachine;

string[] debugCliArgs = { };
var debugCliOption = CLIOption.Parse(debugCliArgs);
var debugGlobalConfig = new GlobalConfig(debugCliOption);
var debugLauncherForm = new LauncherForm(debugGlobalConfig);
var debugMainForm = new MainFormBase(debugLauncherForm);
var debugExecutionContext = new UserScriptExecutionContext(debugGlobalConfig, debugMainForm);

GestureMachineProfile CurrentProfile
    => debugExecutionContext.CurrentProfile;

void DeclareProfile(string profileName) 
    => debugExecutionContext.DeclareProfile(profileName);

IReadOnlyList<GestureMachineProfile> Profiles 
    => debugExecutionContext.Profiles;

readonly SupportedKeys.LogicalKeyDeclaration Keys 
    = debugExecutionContext.Keys;

#region Crevice3 compatible variables.
Crevice.Core.Keys.LogicalSingleThrowKey WheelDown 
    => debugExecutionContext.WheelDown;
Crevice.Core.Keys.LogicalSingleThrowKey WheelUp 
    => debugExecutionContext.WheelUp;
Crevice.Core.Keys.LogicalSingleThrowKey WheelLeft 
    => debugExecutionContext.WheelLeft;
Crevice.Core.Keys.LogicalSingleThrowKey WheelRight 
    => debugExecutionContext.WheelRight;

LogicalSystemKey LeftButton 
    => debugExecutionContext.Keys.LButton;
LogicalSystemKey MiddleButton 
    => debugExecutionContext.Keys.MButton;
LogicalSystemKey RightButton 
    => debugExecutionContext.Keys.RButton;
LogicalSystemKey X1Button 
    => debugExecutionContext.Keys.XButton1;
LogicalSystemKey X2Button 
    => debugExecutionContext.Keys.XButton2;

Crevice.Core.Stroke.StrokeDirection MoveUp 
    => debugExecutionContext.MoveUp;
Crevice.Core.Stroke.StrokeDirection MoveDown 
    => debugExecutionContext.MoveDown;
Crevice.Core.Stroke.StrokeDirection MoveLeft 
    => debugExecutionContext.MoveLeft;
Crevice.Core.Stroke.StrokeDirection MoveRight 
    => debugExecutionContext.MoveRight;
#endregion

IReadOnlyList<SupportedKeys.PhysicalKeyDeclaration> PhysicalKeys 
    => debugExecutionContext.PhysicalKeys;

Crevice.WinAPI.SendInput.SingleInputSender SendInput 
    => debugExecutionContext.SendInput;

void InvokeOnMainThread(Action action)
    => debugMainForm.Invoke(action);

IGestureMachine RootGestureMachine 
    => debugExecutionContext.RootGestureMachine;

UserConfig Config 
    => debugExecutionContext.Config;

Crevice.Core.DSL.WhenElement<EvaluationContext, ExecutionContext> 
    When(Crevice.Core.Context.EvaluateAction<EvaluationContext> func) 
    => debugExecutionContext.When(func);

void Tooltip(string text)
    => debugExecutionContext.Tooltip(text);

void Tooltip(string text, Point point)
    => debugExecutionContext.Tooltip(text, point);

void Tooltip(string text, Point point, int duration)
    => debugExecutionContext.Tooltip(text, point, duration);

void Balloon(string text)
    => debugExecutionContext.Balloon(text);

void Balloon(string text, int timeout)
    => debugExecutionContext.Balloon(text, timeout);

void Balloon(string text, string title)
    => debugExecutionContext.Balloon(text, title);

void Balloon(string text, string title, int timeout)
    => debugExecutionContext.Balloon(text, title, timeout);

void Balloon(string text, string title, ToolTipIcon icon)
    => debugExecutionContext.Balloon(text, title, icon);

void Balloon(string text, string title, ToolTipIcon icon, int timeout)
    => debugExecutionContext.Balloon(text, title, icon, timeout);
