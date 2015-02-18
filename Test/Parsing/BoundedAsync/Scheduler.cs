﻿using System;
using System.Collections.Generic;
using Microsoft.PSharp;

namespace BoundedAsync
{
    [Main]
    internal machine Scheduler
    {
        private Machine Process1;
        private Machine Process2;
        private Machine Process3;
        private int Count;
        private int DoneCounter;

        [Initial]
        private state Init
        {
            entry
            {
                Process1 = create Process { this };
                Process2 = create Process { this };
                Process3 = create Process { this };

                //send eInit { Process2, Process3 } to Process1;
                //send eInit { Process1, Process3 } to Process2;
                //send eInit { Process1, Process2 } to Process3;

                Count = 0;
                DoneCounter = 3;

                raise eUnit;
            }

            on eUnit goto Sync;

            defer eReq;
        }

        private state Sync
        {
            exit
            {
                send eResp to Process1;
                send eResp to Process2;
                send eResp to Process3;
            }

            on eResp goto Sync;
            on eUnit goto Done;

            on eReq do CountReq;
            on eDone do CheckIfDone;
        }

        private state Done
        {
            entry
            {
                delete;
            }
        }

        private action CountReq
        {
            this.Count++;

            if (this.Count == 3)
            {
                this.Count = 0;
                raise eResp;
            }
        }

        private action CheckIfDone
        {
            this.DoneCounter--;

            if (this.DoneCounter == 0)
            {
                delete;
            }
        }
    }
}