// <copyright file="Spreadsheet.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>

// Written by Joe Zachary for CS 3500, September 2013
// Update by Profs Kopta and de St. Germain, Fall 2021, Fall 2024
//     - Updated return types
//     - Updated documentation
///
///<authors> Jared Pratt </authors>
///<version> October 19, 2024 </version>
///

namespace CS3500.Spreadsheet;

using CS3500.Formula;
using CS3500.DependencyGraph;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// <para>
///   Thrown to indicate that a read or write attempt has failed with
///   an expected error message informing the user of what went wrong.
/// </para>
/// </summary>
public class SpreadsheetReadWriteException : Exception
{
    /// <summary>
    ///   <para>
    ///     Creates the exception with a message defining what went wrong.
    ///   </para>
    /// </summary>
    /// <param name="msg"> An informative message to the user. </param>
    public SpreadsheetReadWriteException(string msg)
    : base(msg)
    {
    }
}

/// <summary>
///   <para>
///     Thrown to indicate that a change to a cell will cause a circular dependency.
///   </para>
/// </summary>
public class CircularException : Exception
{
}

/// <summary>
///   <para>
///     Thrown to indicate that a name parameter was invalid.
///   </para>
/// </summary>
public class InvalidNameException : Exception
{
}

/// <summary>
///   <para>
///     An Spreadsheet object represents the state of a simple spreadsheet.  A
///     spreadsheet represents an infinite number of named cells.
///   </para>
/// <para>
///     Valid Cell Names: A string is a valid cell name if and only if it is one or
///     more letters followed by one or more numbers, e.g., A5, BC27.
/// </para>
/// <para>
///    Cell names are case insensitive, so "x1" and "X1" are the same cell name.
///    Your code should normalize (uppercased) any stored name but accept either.
/// </para>
/// <para>
///     A spreadsheet represents a cell corresponding to every possible cell name.  (This
///     means that a spreadsheet contains an infinite number of cells.)  In addition to
///     a name, each cell has a contents and a value.  The distinction is important.
/// </para>
/// <para>
///     The <b>contents</b> of a cell can be (1) a string, (2) a double, or (3) a Formula.
///     If the contents of a cell is set to the empty string, the cell is considered empty.
/// </para>
/// <para>
///     By analogy, the contents of a cell in Excel is what is displayed on
///     the editing line when the cell is selected.
/// </para>
/// <para>
///     In a new spreadsheet, the contents of every cell is the empty string. Note:
///     this is by definition (it is IMPLIED, not stored).
/// </para>
/// <para>
///     The <b>value</b> of a cell can be (1) a string, (2) a double, or (3) a FormulaError.
///     (By analogy, the value of an Excel cell is what is displayed in that cell's position
///     in the grid.) We are not concerned with cell values yet, only with their contents,
///     but for context:
/// </para>
/// <list type="number">
///   <item>If a cell's contents is a string, its value is that string.</item>
///   <item>If a cell's contents is a double, its value is that double.</item>
///   <item>
///     <para>
///       If a cell's contents is a Formula, its value is either a double or a FormulaError,
///       as reported by the Evaluate method of the Formula class.  For this assignment,
///       you are not dealing with values yet.
///     </para>
///   </item>
/// </list>
/// <para>
///     Spreadsheets are never allowed to contain a combination of Formulas that establish
///     a circular dependency.  A circular dependency exists when a cell depends on itself,
///     either directly or indirectly.
///     For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
///     A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
///     dependency.
/// </para>
/// </summary>
public class Spreadsheet
{

    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    /// <summary>
    ///  Dictionary to keep track of non-empty cells.
    ///  The key is the normalized cell name, and the value is the Cell object.
    /// </summary>
    [JsonInclude]
    [JsonPropertyName("Cells")]
    private Dictionary<string, Cell> cells;

    /// <summary>
    /// DependencyGraph to manage dependencies between cells.
    /// </summary>
    private DependencyGraph dependencyGraph;

    //Default constructor

    /// <summary>
    /// Initializes a new instance of the Spreadsheet class.
    /// </summary>
    public Spreadsheet()
    {
        cells = new Dictionary<string, Cell>();
        dependencyGraph = new DependencyGraph();
        Changed = false;
    }

    /// <summary>
    /// Constructs a spreadsheet using the saved data in the file refered to by
    /// the given filename. 
    /// <see cref="Save(string)"/>
    /// </summary>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   Thrown if the file can not be loaded into a spreadsheet for any reason
    /// </exception>
    /// <param name="filename">The path to the file containing the spreadsheet to load</param>
    public Spreadsheet(string filename)
    {
        cells = new Dictionary<string, Cell>();
        dependencyGraph = new DependencyGraph();
        Changed = false;

        try
        {
            string json = File.ReadAllText(filename);
            // The '?' allows for the possibility that deserialization returns null.
            Spreadsheet? sheet = JsonSerializer.Deserialize<Spreadsheet>(json);

            if (sheet != null)
            {
                foreach (var cellEntry in sheet.cells)
                {
                    string name = cellEntry.Key;
                    Cell cell = cellEntry.Value;

                    if (cell == null || cell.StringForm == null) // If the cell or its StringForm is null, treat it as an empty cell
                    {
                        SetContentsOfCell(name, "");
                    }
                    else
                    {
                        SetContentsOfCell(name, cell.StringForm);  // Set the contents of the cell using its StringForm.
                    }
                }
            }
        }
        catch (Exception e)
        {
            throw new SpreadsheetReadWriteException("Error loading spreadsheet: " + e.Message);
        }
        Changed = false;  // After loading, reset the Changed flag to false since the spreadsheet hasn't been modified.
    }


    /// <summary>
    ///   Provides a copy of the normalized names of all of the cells in the spreadsheet
    ///   that contain information (i.e., non-empty cells).
    /// </summary>
    /// <returns>
    ///   A set of the names of all the non-empty cells in the spreadsheet.
    /// </returns>
    public ISet<string> GetNamesOfAllNonemptyCells()
    {
        return new HashSet<string>(cells.Keys);
    }

    /// <summary>
    ///   Returns the contents (as opposed to the value) of the named cell.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   Thrown if the name is invalid.
    /// </exception>
    ///
    /// <param name="name">The name of the spreadsheet cell to query. </param>
    /// <returns>
    ///   The contents as either a string, a double, or a Formula.
    ///   See the class header summary.
    /// </returns>
    public object GetCellContents(string name)
    {
        IsValidCellName(name);
        name = name.ToUpper();

        if (cells.ContainsKey(name))
        {
            return cells[name].Contents;
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    ///  Set the contents of the named cell to the given number.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    ///
    /// <param name="name"> The name of the cell. </param>
    /// <param name="number"> The new contents of the cell. </param>
    /// <returns>
    ///   <para>
    ///     This method returns an ordered list consisting of the passed in name
    ///     followed by the names of all other cells whose value depends, directly
    ///     or indirectly, on the named cell.
    ///   </para>
    ///   <para>
    ///     The order must correspond to a valid dependency ordering for recomputing
    ///     all of the cells, i.e., if you re-evaluate each cells in the order of the list,
    ///     the overall spreadsheet will be correctly updated.
    ///   </para>
    ///   <para>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list [A1, B1, C1] is returned, i.e., A1 was changed, so then A1 must be
    ///     evaluated, followed by B1, followed by C1.
    ///   </para>
    /// </returns>
    private IList<string> SetCellContents(string name, double number)
    {
        dependencyGraph.ReplaceDependees(name, new HashSet<string>());
        cells[name] = new Cell(number);

        IList<string> cellsToRecalculate = GetCellsToRecalculate(name).ToList();
        return cellsToRecalculate;
    }

    /// <summary>
    ///   The contents of the named cell becomes the given text.
    /// </summary>
    ///
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="text"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, string text)
    {
        dependencyGraph.ReplaceDependees(name, new HashSet<string>());
        if (text == "")
        {
            cells.Remove(name);
        }
        else
        {
            cells[name] = new Cell(text);
        }

        IList<string> cellsToRecalculate = GetCellsToRecalculate(name).ToList();
        return cellsToRecalculate;
    }

    /// <summary>
    ///   Set the contents of the named cell to the given formula.
    /// </summary>
    /// <exception cref="InvalidNameException">
    ///   If the name is invalid, throw an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///   <para>
    ///     If changing the contents of the named cell to be the formula would
    ///     cause a circular dependency, throw a CircularException, and no
    ///     change is made to the spreadsheet.
    ///   </para>
    /// </exception>
    /// <param name="name"> The name of the cell. </param>
    /// <param name="formula"> The new contents of the cell. </param>
    /// <returns>
    ///   The same list as defined in <see cref="SetCellContents(string, double)"/>.
    /// </returns>
    private IList<string> SetCellContents(string name, Formula formula)
    {
        IEnumerable<string> variables = formula.GetVariables();
        IEnumerable<string> oldDependees = dependencyGraph.GetDependees(name);
        dependencyGraph.ReplaceDependees(name, variables);

        try
        {
            IList<string> cellsToRecalculate = GetCellsToRecalculate(name).ToList();
            cells[name] = new Cell(formula);
            return cellsToRecalculate;
        }
        catch (CircularException)
        {
            dependencyGraph.ReplaceDependees(name, oldDependees);

            throw;
        }
    }

    /// <summary>
    ///   Returns an enumeration, without duplicates, of the names of all cells whose
    ///   values depend directly on the value of the named cell.
    /// </summary>
    /// <param name="name"> This <b>MUST</b> be a valid name.  </param>
    /// <returns>
    ///   <para>
    ///     Returns an enumeration, without duplicates, of the names of all cells
    ///     that contain formulas containing name.
    ///   </para>
    ///   <para>For example, suppose that: </para>
    ///   <list type="bullet">
    ///      <item>A1 contains 3</item>
    ///      <item>B1 contains the formula A1 * A1</item>
    ///      <item>C1 contains the formula B1 + A1</item>
    ///      <item>D1 contains the formula B1 - C1</item>
    ///   </list>
    ///   <para> The direct dependents of A1 are B1 and C1. </para>
    /// </returns>
    private IEnumerable<string> GetDirectDependents(string name)
    {
        name = name.ToUpper();
        return dependencyGraph.GetDependents(name);
    }

    /// <summary>
    ///   <para>
    ///     This method is implemented for you, but makes use of your GetDirectDependents.
    ///   </para>
    ///   <para>
    ///     Returns an enumeration of the names of all cells whose values must
    ///     be recalculated, assuming that the contents of the cell referred
    ///     to by name has changed.  The cell names are enumerated in an order
    ///     in which the calculations should be done.
    ///   </para>
    ///   <exception cref="CircularException">
    ///     If the cell referred to by name is involved in a circular dependency,
    ///     throws a CircularException.
    ///   </exception>
    ///   <para>
    ///     For example, suppose that:
    ///   </para>
    ///   <list type="number">
    ///     <item>
    ///       A1 contains 5
    ///     </item>
    ///     <item>
    ///       B1 contains the formula A1 + 2.
    ///     </item>
    ///     <item>
    ///       C1 contains the formula A1 + B1.
    ///     </item>
    ///     <item>
    ///       D1 contains the formula A1 * 7.
    ///     </item>
    ///     <item>
    ///       E1 contains 15
    ///     </item>
    ///   </list>
    ///   <para>
    ///     If A1 has changed, then A1, B1, C1, and D1 must be recalculated,
    ///     and they must be recalculated in an order which has A1 first, and B1 before C1
    ///     (there are multiple such valid orders).
    ///     The method will produce one of those enumerations.
    ///   </para>
    ///   <para>
    ///      PLEASE NOTE THAT THIS METHOD DEPENDS ON THE METHOD GetDirectDependents.
    ///      IT WON'T WORK UNTIL GetDirectDependents IS IMPLEMENTED CORRECTLY.
    ///   </para>
    /// </summary>
    /// <param name="name"> The name of the cell.  Requires that name be a valid cell name.</param>
    /// <returns>
    ///    Returns an enumeration of the names of all cells whose values must
    ///    be recalculated.
    /// </returns>
    private IEnumerable<string> GetCellsToRecalculate(string name)
    {
        LinkedList<string> changed = new();
        HashSet<string> visited = [];
        Visit(name, name, visited, changed);
        return changed;
    }

    /// <summary>
    /// A helper method for <see cref="GetCellsToRecalculate"/> that performs a depth-first
    /// search to find all cells that depend on the given cell. It detects circular dependencies
    /// and orders the cells in the correct order for recalculation.
    /// </summary>
    /// <param name="start">
    /// The starting cell from which the recalculation starts. Used to detect circular dependencies.
    /// </param>
    /// <param name="name">
    /// The current cell being visited during the depth-first search.
    /// </param>
    /// <param name="visited">
    /// A set of cells that have already been visited to avoid revisiting them and
    /// to detect circular dependencies.
    /// </param>
    /// <param name="changed">
    /// A linked list that maintains the correct order of cells to be recalculated.
    /// Cells are added to this list after their dependents have been processed.
    /// </param>
    /// <exception cref="CircularException">
    /// Thrown if a circular dependency is detected, i.e., when a cell depends directly or indirectly on itself.
    /// </exception>
    private void Visit(string start, string name, ISet<string> visited, LinkedList<string> changed)
    {
        visited.Add(name);
        foreach (string n in GetDirectDependents(name))
        {
            if (n.Equals(start))
            {
                throw new CircularException();
            }
            // If the dependent cell hasn't been visited yet, recursively visit it
            else if (!visited.Contains(n))
            {
                Visit(start, n, visited, changed);
            }
        }
        // This ensures cells are added in the correct order for recalculation.
        changed.AddFirst(name);
    }

    /// <summary>
    /// Determines whether the specified cell name is valid.
    /// A valid cell name consists of one or more letters followed by one or more digits.
    /// </summary>
    /// <param name="name">The cell name to validate.</param>
    /// <exception 
    /// cref="InvalidNameException">
    /// Thrown if the name is null or invalid.
    /// </exception>
    private void IsValidCellName(string name)
    {
        if (string.IsNullOrEmpty(name) || !IsVar(name))
        {
            throw new InvalidNameException();
        }
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///   <para>
    ///     Return the value of the named cell, as defined by
    ///     <see cref="GetCellValue(string)"/>.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   <see cref="GetCellValue(string)"/>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object this[string name]
    {
        get
        {
            IsValidCellName(name);
            return GetCellValue(name);
        }
    }

    /// <summary>
    /// True if this spreadsheet has been changed since it was 
    /// created or saved (whichever happened most recently),
    /// False otherwise.
    /// </summary>
    [JsonIgnore]
    public bool Changed { get; private set; }

    /// <summary>
    ///   <para>
    ///     Writes the contents of this spreadsheet to the named file using a JSON format.
    ///     If the file already exists, overwrite it.
    ///   </para>
    ///   <para>
    ///     The output JSON should look like the following.
    ///   </para>
    ///   <para>
    ///     For example, consider a spreadsheet that contains a cell "A1" 
    ///     with contents being the double 5.0, and a cell "B3" with contents 
    ///     being the Formula("A1+2"), and a cell "C4" with the contents "hello".
    ///   </para>
    ///   <para>
    ///      This method would produce the following JSON string:
    ///   </para>
    ///   <code>
    ///   {
    ///     "Cells": {
    ///       "A1": {
    ///         "StringForm": "5"
    ///       },
    ///       "B3": {
    ///         "StringForm": "=A1+2"
    ///       },
    ///       "C4": {
    ///         "StringForm": "hello"
    ///       }
    ///     }
    ///   }
    ///   </code>
    ///   <para>
    ///     You can achieve this by making sure your data structure is a dictionary 
    ///     and that the contained objects (Cells) have property named "StringForm"
    ///     (if this name does not match your existing code, use the JsonPropertyName 
    ///     attribute).
    ///   </para>
    ///   <para>
    ///     There can be 0 cells in the dictionary, resulting in { "Cells" : {} } 
    ///   </para>
    ///   <para>
    ///     Further, when writing the value of each cell...
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       If the contents is a string, the value of StringForm is that string
    ///     </item>
    ///     <item>
    ///       If the contents is a double d, the value of StringForm is d.ToString()
    ///     </item>
    ///     <item>
    ///       If the contents is a Formula f, the value of StringForm is "=" + f.ToString()
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="filename"> The name (with path) of the file to save to.</param>
    /// <exception cref="SpreadsheetReadWriteException">
    ///   If there are any problems opening, writing, or closing the file, 
    ///   the method should throw a SpreadsheetReadWriteException with an
    ///   explanatory message.
    /// </exception>
    public void Save(string filename)
    {
        try
        {
            // The JsonSerializer will use the properties and fields marked with [JsonInclude] or [JsonPropertyName].
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filename, json);   // Write the JSON string to the specified file, overwriting it if it already exists.
            Changed = false;
        }
        catch (Exception e)
        {
            throw new SpreadsheetReadWriteException("Error saving spreadsheet" + e.Message);
        }
    }

    /// <summary>
    ///   <para>
    ///     Return the value of the named cell.
    ///   </para>
    /// </summary>
    /// <param name="name"> The cell in question. </param>
    /// <returns>
    ///   Returns the value (as opposed to the contents) of the named cell.  The return
    ///   value should be either a string, a double, or a CS3500.Formula.FormulaError.
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///   If the provided name is invalid, throws an InvalidNameException.
    /// </exception>
    public object GetCellValue(string name)
    {
        IsValidCellName(name);
        name = name.ToUpper();

        if (cells.ContainsKey(name))
        {
            return cells[name].Value;
        }
        return "";
    }

    /// <summary>
    ///   <para>
    ///     Set the contents of the named cell to be the provided string
    ///     which will either represent (1) a string, (2) a number, or 
    ///     (3) a formula (based on the prepended '=' character).
    ///   </para>
    ///   <para>
    ///     Rules of parsing the input string:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       <para>
    ///         If 'content' parses as a double, the contents of the named
    ///         cell becomes that double.
    ///       </para>
    ///     </item>
    ///     <item>
    ///         If the string does not begin with an '=', the contents of the 
    ///         named cell becomes 'content'.
    ///     </item>
    ///     <item>
    ///       <para>
    ///         If 'content' begins with the character '=', an attempt is made
    ///         to parse the remainder of content into a Formula f using the Formula
    ///         constructor.  There are then three possibilities:
    ///       </para>
    ///       <list type="number">
    ///         <item>
    ///           If the remainder of content cannot be parsed into a Formula, a 
    ///           CS3500.Formula.FormulaFormatException is thrown.
    ///         </item>
    ///         <item>
    ///           Otherwise, if changing the contents of the named cell to be f
    ///           would cause a circular dependency, a CircularException is thrown,
    ///           and no change is made to the spreadsheet.
    ///         </item>
    ///         <item>
    ///           Otherwise, the contents of the named cell becomes f.
    ///         </item>
    ///       </list>
    ///     </item>
    ///   </list>
    /// </summary>
    /// <returns>
    ///   <para>
    ///     The method returns a list consisting of the name plus the names 
    ///     of all other cells whose value depends, directly or indirectly, 
    ///     on the named cell. The order of the list should be any order 
    ///     such that if cells are re-evaluated in that order, their dependencies 
    ///     are satisfied by the time they are evaluated.
    ///   </para>
    ///   <example>
    ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
    ///     list {A1, B1, C1} is returned.
    ///   </example>
    /// </returns>
    /// <exception cref="InvalidNameException">
    ///     If name is invalid, throws an InvalidNameException.
    /// </exception>
    /// <exception cref="CircularException">
    ///     If a formula would result in a circular dependency, throws CircularException.
    /// </exception>
    public IList<string> SetContentsOfCell(string name, string content)
    {
        IsValidCellName(name);
        name = name.ToUpper();
        IList<string> cellsToRecalculate = new List<string>();

        if (double.TryParse(content, out double number))
        {
            cellsToRecalculate = SetCellContents(name, number);
        }
        else if (!content.StartsWith("="))
        {
            cellsToRecalculate = SetCellContents(name, (string)content);
        }
        else //(content.StartsWith("="))
        {
            String formula = content.Substring(1);  // Extract the formula string without the '='.
            cellsToRecalculate = SetCellContents(name, new Formula(formula));
        }

        foreach (String cellName in cellsToRecalculate)  // Recalculate the values of the affected cells.
        {
            bool cellVisited = false;
            if (!cellVisited && String.IsNullOrEmpty(content)) // If this is the first cell and the content is empty, skip evaluation.
            {
                cellVisited = true;
                continue;
            }
            if (cells[cellName].Contents is Formula f)
            {
                cells[cellName].Value = f.Evaluate(lookup); // If the cell's contents is a Formula, evaluate it using the lookup delegate.
            }
        }
        Changed = true;
        return cellsToRecalculate;
    }

    /// <summary>
    /// Provides a lookup function for evaluating formulas by retrieving the numeric value of a cell.
    /// </summary>
    /// <param name="cellName">The name of the cell to look up.</param>
    /// <returns>
    /// The numeric value of the specified cell, used during formula evaluation.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the cell does not exist, contains a <see cref="FormulaError"/>,
    /// or does not contain a numeric value.
    /// </exception>
    private double lookup(string cellName)
    {
        if (!cells.ContainsKey(cellName))
        {
            throw new ArgumentException("Cell not found");
        }
        if (cells[cellName].Value is double d)
        {
            return d; // If the cell's value is a double, return it  to be used in the evaluation. 
        }
        else if (cells[cellName].Value is FormulaError)
        {
            throw new ArgumentException("Cell contains a FormulaError");
        }
        else
        {
            throw new ArgumentException("Cell does not contain a numeric value");
        }
    }

    /// <summary>
    /// Represents a cell in the spreadsheet icluding it's contents, value, and stringform.
    /// </summary>
    private class Cell
    {
        public Cell()
        {
            this.Contents = "";
            this.StringForm = "";
            this.Value = "";
        }

        /// <summary>
        /// Gets or sets the contents of the cell.
        /// Can be a string, double, or Formula.
        /// </summary>
        [JsonIgnore]
        public object Contents { get; set; }

        [JsonInclude]
        public string StringForm { get; set; } //Only thing getting serialized

        [JsonIgnore]
        public object Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the Cell class.
        /// </summary>
        /// <param name="contents">The contents of the cell.</param>
        public Cell(object contents)
        {
            this.Contents = contents;
            this.Value = contents;
            this.StringForm = "";

            if (contents is Formula formula)
            {
                StringForm = "=" + formula.ToString();  // If contents is a Formula, set StringForm to "=" followed by the formula string.
            }
            else if (contents is double d)
            {
                StringForm = d.ToString();
                Value = d; //Value is the string itself.
            }
            else if (contents is string s)
            {
                StringForm = s;
                Value = s; // Value is the string itself.
            }
        }
    }
}
