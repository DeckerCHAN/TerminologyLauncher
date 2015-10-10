﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.I18n.TranslationObjects;

namespace TerminologyLauncher.I18n
{
    public static class TranslationFileGenerator
    {
        public static TranslationRoot GenerateAllField()
        {
            var obj = new TranslationRoot() as Object;
            SetFieldValue(ref  obj);
            return obj as TranslationRoot;
        }

        public static void SetFieldValue(ref Object obj)
        {

            var propertyies =
                obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var propertyInfo in propertyies)
            {
                if (propertyInfo.PropertyType.Equals(String.Empty.GetType()))
                {
                    propertyInfo.SetValue(obj, propertyInfo.Name);
                }
                else
                {
                    var valueObj = propertyInfo.GetValue(obj);
                    if (valueObj == null)
                    {
                        valueObj = Activator.CreateInstance(propertyInfo.PropertyType);
                    }
                    SetFieldValue(ref valueObj);
                    propertyInfo.SetValue(obj, valueObj);
                }
            }
        }
    }
}