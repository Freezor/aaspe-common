/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AdminShellNS;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendOperation
{
    #region AasxPackageExplorer

    public static object? AddChild(this IOperation operation, ISubmodelElement? childSubmodelElement, EnumerationPlacmentBase? placement = null)
    {
        // not enough information to select list of children?
        if (childSubmodelElement == null || placement is not EnumerationPlacmentOperationVariable pl)
            return null;

        // ok, use information
        var ov = new OperationVariable(childSubmodelElement);

        childSubmodelElement.Parent = operation;

        switch (pl.Direction)
        {
            case OperationVariableDirection.In:
                operation.InputVariables ??= new List<IOperationVariable>();
                operation.InputVariables.Add(ov);
                break;
            case OperationVariableDirection.Out:
                operation.OutputVariables ??= new List<IOperationVariable>();
                operation.OutputVariables.Add(ov);
                break;
            case OperationVariableDirection.InOut:
                operation.InoutputVariables ??= new List<IOperationVariable>();
                operation.InoutputVariables.Add(ov);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return ov;
    }

    public static EnumerationPlacmentBase? GetChildrenPlacement(this IOperation operation, ISubmodelElement? child)
    {
        // trivial
        if (child == null)
            return null;

        // search
        OperationVariableDirection? dir = null;
        IOperationVariable? opvar = null;
        if (operation.InputVariables != null)
            foreach (var ov in operation.InputVariables.Where(ov => ov?.Value == child))
            {
                dir = OperationVariableDirection.In;
                opvar = ov;
            }

        if (operation.OutputVariables != null)
            foreach (var ov in operation.OutputVariables.Where(ov => ov?.Value == child))
            {
                dir = OperationVariableDirection.Out;
                opvar = ov;
            }

        if (operation.InoutputVariables != null)
            foreach (var ov in operation.InoutputVariables.Where(ov => ov?.Value == child))
            {
                dir = OperationVariableDirection.InOut;
                opvar = ov;
            }

        // found
        if (!dir.HasValue)
            return null;
        return new EnumerationPlacmentOperationVariable()
        {
            Direction = dir.Value,
            OperationVariable = opvar as OperationVariable
        };
    }

    public static List<IOperationVariable>? GetVars(this IOperation op, OperationVariableDirection dir)
    {
        return dir switch
        {
            OperationVariableDirection.In => op.InputVariables,
            OperationVariableDirection.Out => op.OutputVariables,
            _ => op.InoutputVariables
        };
    }

    public static List<IOperationVariable> SetVars(
        this IOperation op, OperationVariableDirection dir, List<IOperationVariable> value)
    {
        switch (dir)
        {
            case OperationVariableDirection.In:
                op.InputVariables = value;
                return op.InputVariables;
            case OperationVariableDirection.Out:
                op.OutputVariables = value;
                return op.OutputVariables;
            case OperationVariableDirection.InOut:
            default:
                op.InoutputVariables = value;
                return op.InoutputVariables;
        }
    }

    #endregion

    public static IOperation UpdateFrom(
        this IOperation elem, ISubmodelElement? source)
    {
        if (source == null)
            return elem;

        ((ISubmodelElement)elem).UpdateFrom(source);

        if (source is SubmodelElementCollection {Value: not null} srcColl)
        {
            var operationVariables = srcColl.Value.Copy().Select(
                (isme) => new OperationVariable(isme)).ToList();
            elem.InputVariables = operationVariables.ConvertAll(op => (IOperationVariable)op);
        }

        if (source is not SubmodelElementCollection {Value: not null} srcList) return elem;
        {
            var operationVariables = ExtendISubmodelElement.Copy(srcList.Value).Select(
                (isme) => new OperationVariable(isme)).ToList();
            elem.InputVariables = operationVariables.ConvertAll(op => (IOperationVariable)op);
        }

        return elem;
    }
}