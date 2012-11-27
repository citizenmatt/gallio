// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;

namespace Gallio.Common.Remoting
{
    /// <summary>
    /// Abstract base class for client channels implemented using the .Net
    /// remoting infrastructure.
    /// </summary>
    public abstract class BaseServerChannel : BaseChannel, IServerChannel
    {
        private readonly List<MarshalByRefObject> remoteComponents;

        /// <summary>
        /// Creates a channel.
        /// </summary>
        /// <param name="channel">The .Net remoting channel.</param>
        /// <param name="channelUri">The root Uri associated with the channel.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="channel"/> or <paramref name="channelUri"/> is null.</exception>
        protected BaseServerChannel(IChannel channel, Uri channelUri)
            : base(channel, channelUri)
        {
            remoteComponents = new List<MarshalByRefObject>();
        }

        /// <inheritdoc />
        public void RegisterService(string serviceName, MarshalByRefObject component)
        {
            if (serviceName == null)
                throw new ArgumentNullException("serviceName");
            if (component == null)
                throw new ArgumentNullException("component");

            RemotingServices.Marshal(component, serviceName);

            lock (remoteComponents)
                remoteComponents.Add(component);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (remoteComponents)
                {
                    foreach (MarshalByRefObject component in remoteComponents)
                        RemotingServices.Disconnect(component);
                    remoteComponents.Clear();
                }
            }

            base.Dispose(disposing);
        }
    }
}
