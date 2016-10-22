﻿/*******************************************************************************************************************//**
 * @file Character.cs
 **********************************************************************************************************************/

using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSmashPolls.World_Control;

namespace SuperSmashPolls.Characters {

    /***************************************************************************************************************//**
     * TODO Test this class
     * This class will hold textures for characters, their moves, their effects, other characters they encounter, etc.
     * @note Character's moves need to be established by an object of the PhysicsAffect class.
     * @note Character position is not determined by this class.
     * @note Actual characters need to be declared as constants in an instance of this class.
     ******************************************************************************************************************/ 
    class Character : ObjectClass {
        /** The thumbnail of the character to show on the character selection screen */
        private Texture2D Thumbnail;
        /** The movement speed of this character */
        public WorldUnit MovementSpeed;

        /***********************************************************************************************************//**
         * Constructor
         **************************************************************************************************************/
        public Character(WorldUnit drawPosition, int weight, WorldUnit size, bool solid = false) 
            : base(drawPosition, weight, size, solid) {
        }


    }

}
