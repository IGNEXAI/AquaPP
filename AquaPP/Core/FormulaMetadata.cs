using System;
using System.Collections.Generic;

namespace AquaPP.Core;

// Class to store metadata about a formula.  Making it immutable for thread safety
public class FormulaMetadata
{
    // Public properties to store formula information.  Using properties is preferred in C#
    public string Name { get; } // Name of the formula (e.g., "Darcy-Weisbach")
    public string Author { get; private set; } // Author(s) of the formula
    public string Reference { get; private set; } // Source reference (e.g., book, journal article)
    public int Year { get; private set; } // Year of publication
    public string Description { get; private set; } // A brief description of the formula's purpose
    public string Notes { get; private set; } //any specific notes about the formula

    // Constructor to initialize the formula metadata.  Using a constructor enforces that all data is provided.
    public FormulaMetadata(string name, string author, string reference, int year, string description, string notes)
    {
        // Input validation:  Important for data integrity.  Throw exceptions for bad data.
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Formula name cannot be null or empty.", nameof(name));
        }

        if (string.IsNullOrEmpty(author))
        {
            throw new ArgumentException("Formula author cannot be null or empty.", nameof(author));
        }

        if (string.IsNullOrEmpty(reference))
        {
            throw new ArgumentException("Formula reference cannot be null or empty.", nameof(reference));
        }

        if (year <= 0) // Basic year validation
        {
            throw new ArgumentException("Year must be a positive value.", nameof(year));
        }

        if (string.IsNullOrEmpty(description))
        {
            throw new ArgumentException("Formula description cannot be null or empty", nameof(description));
        }

        Name = name;
        Author = author;
        Reference = reference;
        Year = year;
        Description = description;
        Notes = notes;
    }

    // Override ToString() for easy display in UI or logging.  This is very helpful.
    public override string ToString()
    {
        return $"{Name} by {Author} ({Year}). {Description} Reference: {Reference}. Notes: {Notes}";
    }

    //Added an equals override.
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        FormulaMetadata other = (FormulaMetadata)obj;
        return (Name == other.Name &&
                Author == other.Author &&
                Reference == other.Reference &&
                Year == other.Year &&
                Description == other.Description &&
                Notes == other.Notes);
    }

    //Override GetHashCode()
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            hash = hash * 23 + Name.GetHashCode();
            hash = hash * 23 + Author.GetHashCode();
            hash = hash * 23 + Reference.GetHashCode();
            hash = hash * 23 + Year.GetHashCode();
            hash = hash * 23 + Description.GetHashCode();
            hash = hash * 23 + Notes.GetHashCode();
            return hash;
        }
    }
}

// Class to manage a collection of FormulaMetadata objects.  This will make it easier to store and retrieve them.
public class FormulaLibrary
{
    private readonly List<FormulaMetadata>
        _formulas = new List<FormulaMetadata>(); // Use readonly and private for encapsulation

    // Method to add a formula to the library.  Consider adding checks for duplicates.
    public void AddFormula(FormulaMetadata formula)
    {
        if (formula == null)
        {
            throw new ArgumentNullException(nameof(formula), "FormulaMetadata object cannot be null.");
        }

        if (_formulas.Contains(formula))
        {
            throw new ArgumentException("Formula already exists in the library.", nameof(formula));
        }

        _formulas.Add(formula);
    }

    // Method to get a formula by its name.  Added a way to search by name.
    public FormulaMetadata GetFormulaByName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Formula name cannot be null or empty.", nameof(name));
        }

        // Use Find method for more efficient searching.
        return _formulas.Find(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    //Method to get all the formulas
    public List<FormulaMetadata> GetAllFormulas()
    {
        return new List<FormulaMetadata>(_formulas); // Returns a copy to prevent external modification.
    }
}