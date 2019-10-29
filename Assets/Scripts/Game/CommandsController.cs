﻿using System;
using System.Collections.Generic;
using System.Linq;

public class CommandsController
{
    public Queue<BaseCommand> CommandList { get; set; } = new Queue<BaseCommand>();
    public bool CommandsRunning { get; private set; }

    private List<CommandsMethods> _allCommands;
    private BaseCommand _runningCommand;
    private Character _implementator;

    private int currentCommandIndex;
    private List<string> stringCommandList;

    public CommandsController(Character implementator)
    {
        _implementator = implementator;
        _allCommands = Enum.GetValues(typeof(CommandsMethods)).Cast<CommandsMethods>().ToList();
    }

    public bool TryGetCommand(string commandString, out BaseCommand command)
    {
        command = GetSimpleCommand(commandString);

        if(command != null)
            return true;
        return false;
    }

    public void SetCommandsList(List<string> commandList)
    {
        stringCommandList = commandList;

        for (currentCommandIndex = 0; currentCommandIndex < commandList.Count; currentCommandIndex++)
        {
            var command = GetSimpleCommand(commandList[currentCommandIndex]);
            if (command != null)
                CommandList.Enqueue(command);

        }
    }

    private BaseCommand GetSimpleCommand(string command)
    {
        string[] arguments = null;
        string methodName = GetMethodSingature(command, ref arguments);

        CommandsMethods commandEnum = _allCommands.FirstOrDefault(com => com.ToString() == methodName);

        if (commandEnum == 0)
            return null;

        switch (commandEnum)
        {
            case (CommandsMethods.Forward): return new MoveCommand(_implementator, MoveSides.FORWARD);
            case (CommandsMethods.Backward): return new MoveCommand(_implementator, MoveSides.BACKWARD);
            case (CommandsMethods.Turn):
                {
                    if (arguments == null || arguments.Length == 0)
                        return null;

                    var turnSide = arguments[0].ToLower();

                    if (turnSide == TurnArguments.Right.ToString().ToLower())
                        return new RotateCommand(_implementator, TurnArguments.Right);
                    else if (turnSide == TurnArguments.Left.ToString().ToLower())
                        return new RotateCommand(_implementator, TurnArguments.Left);
                    return null;
                }
            case (CommandsMethods.Do):
                {
                    if (arguments.Length == 0)
                        return null;

                    int count = Convert.ToInt32(arguments[0]);
                    return new DoCommand(_implementator, null, count);
                }
            case (CommandsMethods.While):
                {
                    if (arguments.Length == 0)
                        return null;



                    break;
                }
        }

        return null;
    }

    //private BaseCommand GetCommand(string command)
    //{
    //    string[] arguments = null;
    //    string methodName = GetMethodSingature(command, ref arguments);

    //    CommandsMethods commandEnum = _allCommands.First(com => com.ToString() == methodName);

    //    switch (commandEnum)
    //    {
    //        case (CommandsMethods.Forward): return new MoveCommand(_implementator, MoveSides.FORWARD);
    //        case (CommandsMethods.Backward): return new MoveCommand(_implementator, MoveSides.BACKWARD);
    //        case (CommandsMethods.Turn):
    //            {
    //                if (arguments.Length == 0)
    //                    return null;

    //                var turnSide = arguments[0].ToLower();

    //                if (turnSide == TurnArguments.Right.ToString().ToLower())
    //                    return new RotateCommand(_implementator, TurnArguments.Right);
    //                else if (turnSide == TurnArguments.Left.ToString().ToLower())
    //                    return new RotateCommand(_implementator, TurnArguments.Left);
    //                return null;
    //            }
    //        case (CommandsMethods.Do):
    //            {
    //                if (arguments.Length == 0)
    //                    return null;

    //                int count = Convert.ToInt32(arguments[0]);

    //                var listOfDoCommand = new List<BaseCommand>();

    //                currentCommandIndex++;
    //                while (stringCommandList[currentCommandIndex] != "End()" && currentCommandIndex < stringCommandList.Count)
    //                {
    //                    var com = GetCommand(stringCommandList[currentCommandIndex]);
    //                    currentCommandIndex++;
    //                    if (com != null)
    //                        listOfDoCommand.Add(com);
    //                }

    //                return new DoCommand(_implementator, listOfDoCommand, count);
    //            }
    //        case (CommandsMethods.While):
    //            {
    //                if (arguments.Length == 0)
    //                    return null;



    //                break;
    //            }
    //    }

    //    return null;
    //}

    private string GetMethodSingature(string command, ref string[] arguments)
    {
        int methodParametersStart = command.IndexOf('(');
        int methodParametersFinish = command.IndexOf(')');

        if (methodParametersStart == -1  || methodParametersStart >= methodParametersFinish)
            return command;

        // +1 ; -1 remove symbols '(' and ')'
        arguments = command.Substring(methodParametersStart + 1, methodParametersFinish - methodParametersStart - 1)
            .Replace(" ", string.Empty)
            .Split(',');
        return command.Substring(0, methodParametersStart);
    }

    public void RunCommands()
    {
        if (_runningCommand != null && _runningCommand.isRunning)
            return;

        if (CommandList.Count == 0)
        {
            CommandsRunning = false;
            return;
        }

        _runningCommand = CommandList.Dequeue();
        _runningCommand.Execute();
    }

    public void StartCommands()
    {
        CommandsRunning = true;
    }

    public void Clear()
    {
        CommandList = new Queue<BaseCommand>();
    }
}
