
# Gesture DSL

## When

All gesture definition starts from `When` clause, representing the condition for the activation of a gesture. And also `When` clause is the first context of a gesture.
```cs
var Chrome = When(ctx =>
{
    return ctx.ForegroundWindow.ModuleName == "chrome.exe";
});
```

_Note: `ctx` is EvaluationContext, see [Crevice4 Core API/EvaluationContext](#evaluationcontext) for more details._

The next to `When` are `On` and `OnDecomposed` clauses.

## On 

`On` clause takes a button or a sequence of stroke as it's argument. This clause can be declared successively if you needed. So, `On` clause is the second or later context of a gesture. 

```cs
// Button gesture.
Chrome.
On(Keys.RButton). // If you press mouse's right button,
```

```cs
// Button gesture with two buttons.
Chrome.
On(Keys.RButton). // If you press mouse's right button,
On(Keys.LButton). // and press mouse's left button,
```

```cs
// Storke gesture.
Chrome.
On(Keys.RButton). // If you press mouse's right button,
On(Keys.MoveUp, Keys.MoveDown). // and draw stroke to up and to down by the pointer,
```

Other than itself, this clause takes `Press`, `Do`, and `Release` clauses as the next clause. 

```cs
Chrome.
On(Keys.RButton).
Press(ctx => {});
```
```cs
Chrome.
On(Keys.RButton).
Do(ctx => {});
```
```cs
Chrome.
On(Keys.RButton).
Release(ctx => {});
```

And also, these clauses are can be declared at the same time.

```cs
Chrome.
On(Keys.RButton).
Press(ctx => {}).
Do(ctx => {}).
Release(ctx => {});
```

##### Grammatical limitations: { ignore=true }
* `On` and `OnDecomposed` clauses given the same button **can not** be declared on the same context. See [OnDecomposed](#ondecomposed) for more details.

## OnDecomposed

`OnDecomposed` clause takes a button as It's argument. Like `On`, `OnDecomposedOn` clause is the second or later context of a gesture, too. But, in contrast to `On` clause, this clause **can not** be declared successively, and **can not** take `Do` clause as the next clause. This clause exists for the cases that you want to simply hook the press and release events to an action. This clause takes `Press` and `Release` clauses as the next clause. These clauses will directly be connected to the action, and if a button pressed, each of all the events published while it will invoke it.

```cs
Chrome.
OnDecomposed(Keys.RButton). // If you press and hold mouse's right button,
Press(ctx => 
{
    // then this code will be executed, each time the press event will be published.
}). 
Release(ctx => {});
```

```cs
// This clause CAN NOT be declared successively.
Chrome.
OnDecomposed(Keys.RButton). 
OnDecomposed(Keys.MButton). // Compilation error.
```

```cs
// This clause CAN NOT be declared on the same context with `On` clause given the same button.
Chrome.
On(Keys.RButton).
OnDecomposed(Keys.RButton). // Runtime error will be thrown and warning messsage will be shown.
```


##### Grammatical limitations: { ignore=true }
* `OnDecomposed` clause does not have `Do()` functions.
* `On` and `OnDecomposed` clauses given the same button **can not** be declared on the same context.

## Do

`Do` clause declares an action which will be executed only when the conditions, given by the context it to be declared, are fully filled. `Do` clause is the last context of a gesture. 

```cs
On(Keys.RButton). // If you press mouse's right button,
Do(ctx => // and release mouse's right button,
{
    // then this code will be executed.
});
```

_Note: `ctx` is `ExecutionContext`, see [Crevice4 Core API/ExecutionContext](#executioncontext) for more details._

## Press

`Press` clause declares an action which will be executed only when the conditions, given by the context it to be declared, are fully filled, except for the last clause's release event. `Press` clause is the last context of a gesture. 

```cs
On(Keys.RButton). // If you press mouse's right button,
Press(ctx => // without waiting for release event,
{
    // then this code will be executed.
});
```

_Note: `ctx` is `ExecutionContext`, see [Crevice4 Core API/ExecutionContext](#executioncontext) for more details._

## Release

`Release` clause declares an action which will be executed only when the conditions, given by the context it to be declared, are fully filled. `Release` clause is the last context of a gesture. 

```cs
On(Keys.RButton). // If you press mouse's right button,
Release(ctx => // and release mouse's right button,
{
    // then this code will be executed.
});
```

_Note: `ctx` is `ExecutionContext`, see [Crevice4 Core API/ExecutionContext](#executioncontext) for more details._

## Button gesture
As you may know, mouse gestures with it's buttons are called "rocker gesture" in mouse gesture utility communities. But we call it, including it with keyboard's keys, simply `Button gesture` here. 

```cs
// Button gesture.
Chrome.
On(Keys.RButton). // If you press mouse's right button,
Do(ctx => // and release mouse's right button,
{
    // then this code will be executed.
});
```

```cs
// Button gesture with two buttons.
Chrome.
On(Keys.RButton). // If you press mouse's right button,
On(Keys.LButton). // and press mouse's left button,
Do(ctx => // and release mouse's left or right button,
{
    // then this code will be executed.
});
```

Even if after pressing a button which means the start of a gesture, you can cancel it by holding the button pressing until it to be timeout.

```cs
Chrome.
On(Keys.RButton). // If you WRONGLY pressed mouse's right button,
Do(ctx => // you hold the button until it to be timeout and release it,
{
    // then this code will NOT be executed.
});
```

This means actions declared in `Do` clause is not assured it's execution.

Above three gestures are `Button gesture` by the standard buttons. `On` clause with standard buttons can be used for declare `Do` clause but also `Press` and `Release` clauses.

### Button gesture with Press/Release

`Do` clause is just simple but there are cases do not fit to use it. For example, where there is need to hook to the press or release event of a button. `Press` and `Release` clauses fit to this case. These can be written just after `On` clause.

```cs
// Convert Keys.XButton1 to Keys.LWin.
Chrome.
On(Keys.XButton1).
Press(ctx =>
{
    SendInput.ExtendedKeyDown(Keys.LWin);
}).
Release(ctx =>
{
    SendInput.ExtendedKeyUp(Keys.LWin);
});
```

For `Release` clause, it can be after `Do` clause.

```cs
Chrome.
On(Keys.XButton2).
Press(ctx =>
{
    // Assured.
}).
Do(ctx =>
{
    // Not assured. 
    // e.g. When the gesture to be timeout,
    //      this action will not be executed.
}).
Release(ctx =>
{
    // Assured.
});
```

Actions declared in `Press` and `Release` clauses are different from it of `Do` clause, the execution of these are assured.

_Note: Be careful that this conversion is incomplete. See [Convert a button into an arbitrary button](#convert-a-button-into-an-arbitrary-button) for more details._

### Button gesture with single state button

Few of the buttons in `Keys` are different from the standard buttons; these have only one state, and only one event. So, `On` clauses with these can not be used with `Press` and `Release` clauses.

```cs
Chrome.
On(Keys.WheelUp).
Press(ctx => { }); // Compilation error
```

```cs
Chrome.
On(Keys.WheelUp).
Do(ctx => { }); // OK
```

```cs
Chrome.
On(Keys.WheelUp).
Release(ctx => { }); // Compilation error
```


##### Grammatical limitations: { ignore=true }
* `On` clause with single state button does not have `Press()` and `Release()` functions.

Single state buttons are `Keys.WheelUp`,  `Keys.WheelDown`,  `Keys.WheelLeft`, and  `Keys.WheelRight`.

## Stroke gesture

"Mouse gestures by strokes", namely `Stroke gesture` is the most important part in the functions of mouse gesture utilities.

`On` clause takes arguments that consist of combination of `Keys.MoveUp`, `Keys.MoveDown`, `Keys.MoveLeft` and `Keys.MoveRight`. These are representing directions of movements of the mouse pointer.

```cs
Chrome.
On(Keys.RButton). // If you press right button,
On(Keys.MoveDown, Keys.MoveRight). // and draw stroke to down and to right by the pointer,
Do(ctx => // and release right button,
{
    SendInput.Multiple().
    ExtendedKeyDown(Keys.ControlKey).
    ExtendedKeyDown(Keys.W).
    ExtendedKeyUp(Keys.W).
    ExtendedKeyUp(Keys.ControlKey).
    Send(); // then send Ctrl+W to Chrome.
});
```

`Stroke gesture` represents special case when a standard button is pressed, so it have the same grammatical limitation to `Button gesture with single state button`.


```cs
Chrome.
On(Keys.RButton).
On(Keys.MoveDown).
Press(ctx => { }); // Compilation error
```

```cs
Chrome.
On(Keys.RButton).
On(Keys.MoveDown).
Do(ctx => { }); // OK
```

```cs
Chrome.
On(Keys.RButton).
On(Keys.MoveDown).
Release(ctx => { }); // Compilation error
```


##### Grammatical limitations: { ignore=true }
* `On` clause with `Keys.Move*` does not have `Press()` and `Release()` functions.
* `On` clause with `Keys.Move*` should have `Button gesture` by standard button as the previous context.
* `On` clause with `Keys.Move*` should be the last element of the sequence of `On` clauses.
