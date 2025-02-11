﻿using Barotrauma.Networking;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using System.Xml.Linq;

namespace Barotrauma.Items.Components
{
    partial class Pump : Powered, IServerSerializable, IClientSerializable
    {
        public void ServerEventRead(IReadMessage msg, Client c)
        {
            float newFlowPercentage = msg.ReadRangedInteger(-10, 10) * 10.0f;
            bool newIsActive = msg.ReadBoolean();

            if (item.CanClientAccess(c))
            {
                if (newFlowPercentage != FlowPercentage)
                {
                    GameServer.Log(GameServer.CharacterLogName(c.Character) + " set the pumping speed of " + item.Name + " to " + (int)(newFlowPercentage) + " %", ServerLog.MessageType.ItemInteraction);
                }
                if (newIsActive != IsActive)
                {
                    GameServer.Log(GameServer.CharacterLogName(c.Character) + (newIsActive ? " turned on " : " turned off ") + item.Name, ServerLog.MessageType.ItemInteraction);
                }
                if (pumpSpeedLockTimer <= 0.0f)
                {
                    TargetLevel = null;
                }

                FlowPercentage = newFlowPercentage;
                IsActive = newIsActive;
            }

            //notify all clients of the changed state
            item.CreateServerEvent(this);
        }

        public void ServerEventWrite(IWriteMessage msg, Client c, NetEntityEvent.IData extraData = null)
        {
            //flowpercentage can only be adjusted at 10% intervals -> no need for more accuracy than this
            msg.WriteRangedInteger((int)(flowPercentage / 10.0f), -10, 10);
            msg.Write(IsActive);
            msg.Write(Hijacked);
            if (TargetLevel != null)
            { 
                msg.Write(true);
                msg.Write(TargetLevel.Value);
            }
            else
            {
                msg.Write(false);
            }
        }
    }
}
