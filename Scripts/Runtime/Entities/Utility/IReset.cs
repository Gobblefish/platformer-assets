/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Entities.Utility {

    public interface IReset {
        void OnStartResetting();
        void OnFinishResetting();
    }

}

