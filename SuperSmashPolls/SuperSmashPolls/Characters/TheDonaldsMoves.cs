﻿#define OLD_MOVES
#undef OLD_MOVES

#if OLD_MOVES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace SuperSmashPolls.Characters {


    class TheDonaldsMoves : Moves {

        /// <summary>
        /// Builds a wall. This handles the creation of the body and the forces of the wall.
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public override void Special(Character character) {

            var DetectedPoints = UseMove(SpecialIndex, ref character);

            if (DetectedPoints.Count < 1)
                return;

            //TODO code for move

            PlaySound(SpecialIndex);

        }

        /// <summary>
        /// Creates an explosion outward from TheDonald. This is a linear explosion from the center of TheDonald's body,
        /// with a radius of 4m and a force of 300, maxforce of the max value of a float. This explosion does not affect
        /// TheDonald.
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public override void SideSpecial(Character character) {

            var DetectedPoints = UseMove(SideSpecialIndex, ref character);

            if (DetectedPoints.Count < 1)
                return;

            foreach (var i in DetectedPoints) {

                SimpleExplosion Explosion = new SimpleExplosion(character.GameWorld) {
                    Power = 1,
                    DisabledOnGroup = character.CharacterBody.CollisionGroup
                };

                Explosion.Activate(i[0].LocalPoint, 4, 300); //Test this

            }

            PlaySound(SideSpecialIndex);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public override void UpSpecial(Character character) {

            throw new NotImplementedException();

            PlaySound(UpSpecialIndex);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public override void DownSpecial(Character character) {

            throw new NotImplementedException();

            PlaySound(DownSpecialIndex);

        }

        /// <summary>
        /// Impliments the basic punch
        /// </summary>
        /// <param name="character">The character preforming the move</param>
        public override void BasicAttack(Character character) {

            BasicPunch(character);

            PlaySound(BasicIndex);

        }

    }

}

#endif