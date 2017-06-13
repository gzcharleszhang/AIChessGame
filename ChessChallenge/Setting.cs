/*
 * Charles, Nemo, Henry
 * ISU
 * JAn 25, 2017
 * 
 * Setting Class
 * creates a setting object
 * stores theme and music
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChessChallenge
{
    public class Setting
    {
        // stores a theme that determines the pieces' colours
        private Theme _theme;

        /// <summary>
        /// Allows theme to be visible to other classes
        /// </summary>
        public Theme Theme
        {
            get
            {
                return _theme;
            }
            // theme can be changed externally
            set
            {
                _theme = value;
            }
        }

        /// <summary>
        /// Allows music to be visible to oter classes
        /// </summary>
        public Stream Music
        {
            get
            {
                // Returns the chillychess music if the theme is chillychess
                if (Theme == Theme.ChillyChess)
                {
                    return Properties.Resources._1;
                }
                // Returns the classic music if the theme is classic
                else if (Theme == Theme.Classic)
                {
                    return Properties.Resources._0;
                }
                // Retursn the dusty music if the theme is dusty
                else if (Theme == Theme.Dusty)
                {
                    return Properties.Resources._2;
                }
                // Retursn the rainbow music
                else
                {
                    return Properties.Resources._3;
                }

            }
        }
    }
}
