using UnityEngine;

namespace Mekaiju
{

    public static class TransformExtension
    {

        public static Transform FindNested(this Transform self, string name)
        {
            Transform t_tr = null;
            foreach(Transform tr in self)
            {
                if (tr.name == name)
                    t_tr = tr;
                else
                    t_tr = FindNested(tr, name);

                if (t_tr)
                    break;
            }

            return t_tr;
        }

    }

}
