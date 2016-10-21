﻿/*******************************************************************************************************************//**
 * @file PlayerClass.cd
 **********************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SuperSmashPolls.Characters;
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls.GameItemControl {

    /***************************************************************************************************************//**
     * Class to control the movment and interaction of players.
     * @note This class should inlcude an instance of the character class, and should not repeat any affects of that 
     * class.
     ******************************************************************************************************************/
    class PlayerClass : ObjectClass {

        /***********************************************************************************************************//**
         * TODO SetCharacter method
         **************************************************************************************************************/

        /***********************************************************************************************************//**
         * Constructor
         **************************************************************************************************************/
        public PlayerClass(WorldUnit drawPosition, int weight, WorldUnit size, bool solid = false) 
            : base(drawPosition, weight, size, solid) {
        }

        /***********************************************************************************************************//**
         * Updates the character
         **************************************************************************************************************/

    }

}
