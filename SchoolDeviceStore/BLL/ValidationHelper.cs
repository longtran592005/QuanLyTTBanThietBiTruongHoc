using System;
using System.Collections.Generic;
using System.Linq;
using DTO;

namespace BLL
{
    public static class ValidationHelper
    {
        public static string RequireText(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(fieldName + " is required.");

            return value.Trim();
        }

        public static int RequireNonNegativeInt(int value, string fieldName)
        {
            if (value < 0)
                throw new ArgumentException(fieldName + " cannot be negative.");

            return value;
        }

        public static decimal RequireNonNegativeDecimal(decimal value, string fieldName)
        {
            if (value < 0m)
                throw new ArgumentException(fieldName + " cannot be negative.");

            return value;
        }

        public static void RequireDuplicateFree(bool exists, string fieldName, string value)
        {
            if (exists)
                throw new ArgumentException($"A record with {fieldName} '{value}' already exists.");
        }

        public static void RequireNotNull(object value, string fieldName)
        {
            if (value == null)
                throw new ArgumentException(fieldName + " is required.");
        }
    }
}