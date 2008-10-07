using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace CSharpRecipes.Config
{
    /// <summary>
    /// Configuration section for the replication installation agent
    /// </summary>
    public class CSharpRecipesConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CSharpRecipesConfigurationSection() 
        {
        }

        [ConfigurationProperty("Chapters",
            IsDefaultCollection = false)]
        public ChapterConfigurationElementCollection Chapters
        {
            get
            {
                ChapterConfigurationElementCollection  chapterCollection =
                (ChapterConfigurationElementCollection )base["Chapters"];
                return chapterCollection;
            }
        }

        [ConfigurationProperty("Editions",
            IsDefaultCollection = false)]
        public EditionConfigurationElementCollection Editions
        {
            get
            {
                EditionConfigurationElementCollection editionCollection =
                (EditionConfigurationElementCollection )base["Editions"];
                return editionCollection;
            }
        }

        [ConfigurationProperty("CurrentEdition", DefaultValue = "3rd edition")]
        public string CurrentEdition
        {
            get
            {
                return (string)this["CurrentEdition"];
            }
        }
    }
}
