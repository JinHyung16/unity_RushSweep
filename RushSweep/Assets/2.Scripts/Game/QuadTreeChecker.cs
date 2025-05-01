using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hugh.Game
{
    public interface IQuadTreeCheckerNode<T> where T : IQuadTreeCheckerNode<T>
    {
        Vector3 GetPosition();
        bool NeedCheck();
    }

    public class QuadTreeChecker<T> where T : IQuadTreeCheckerNode<T>
    {
        public class Node
        {
            Node[] _children;
            List<T> _elements;
            public float _left;
            public float _right;
            public float _top;
            public float _bottom;

            public Node()
            {
                _children = new Node[4];
                _elements = new List<T>(100);
            }

            public void Clear(Stack<Node> pool, bool returnPool)
            {
                for (int ii = 0; ii < 4; ++ii)
                {
                    if (null != _children[ii])
                    {
                        _children[ii].Clear(pool, true);
                        _children[ii] = null;
                    }
                }

                _elements?.Clear();
                if (returnPool)
                {
                    pool.Push(this);
                }
            }

            public void SetRect(float left, float right, float top, float bottom)
            {
                _left = left;
                _right = right;
                _top = top;
                _bottom = bottom;
            }

            public void Push(T element, Stack<Node> pool, float minimum, int density)
            {
                if (_children[0] != null)
                {
                    var pos = AdjustPosition(element.GetPosition());

                    for (int ii = 0; ii < 4; ++ii)
                    {
                        if (_children[ii]._left <= pos.x && pos.x <= _children[ii]._right &&
                            _children[ii]._bottom <= pos.z && pos.z <= _children[ii]._top)
                        {
                            _children[ii].Push(element, pool, minimum, density);
                            return;
                        }
                    }
                    return;
                }

                if (density <= _elements.Count && minimum < (_right - _left))
                {
                    Devide(pool, minimum, density);
                    Push(element, pool, minimum, density);
                    return;
                }

                _elements.Add(element);
            }
            public void Devide(Stack<Node> pool, float minimum, int density)
            {
                for (int ii = 0; ii < 4; ++ii)
                {
                    Node newNode = pool.Count > 0 ? pool.Pop() : new Node();
                    newNode.SetRect(
                        ii % 2 == 0 ? _left : _left + (_right - _left) / 2,
                        ii % 2 == 0 ? _left + (_right - _left) / 2 : _right,
                        ii / 2 == 0 ? _bottom + (_top - _bottom) / 2 : _top,
                        ii / 2 == 0 ? _bottom : _bottom + (_top - _bottom) / 2
                    );
                    _children[ii] = newNode;
                }

                foreach (var element in _elements)
                {
                    Push(element, pool, minimum, density);
                }

                _elements?.Clear();
            }
            public void Check(Func<T, T, bool> check)
            {
                for (int ii = 0; ii < 4; ++ii)
                {
                    if (null != _children[ii])
                    {
                        _children[ii].Check(check);
                    }
                }

                for (int ii = 0; ii < _elements.Count; ++ii)
                {
                    for (int jj = ii + 1; jj < _elements.Count; ++jj)
                    {
                        if (check(_elements[ii], _elements[jj]))
                        {
                        }
                    }
                }
            }

            public void DrawGizmos()
            {
                //Vector3 center = new Vector3((Left + Right) / 2.0f, 0.5f, (_top + _bottom) / 2.0f);
                //Vector3 size = new Vector3(Right - Left, 1.0f, _top - _bottom);
                //
                //Gizmos.color = Color.yellow;
                //Gizmos.DrawWireCube(center, size);

                for (int ii = 0; ii < 4; ++ii)
                {
                    if (null != _children[ii])
                    {
                        _children[ii].DrawGizmos();
                    }
                }
            }

            private Vector3 AdjustPosition(Vector3 pos)
            {
                return new Vector3(pos.x, pos.y, pos.z);
            }
        }

        Node _root;
        Stack<Node> _nodePool;
        List<T> _checkList;
        float _left;
        float _right;
        float _top;
        float _bottom;
        float _minimum;
        int _density;
        Func<T, T, bool> _checkFunc;

        public QuadTreeChecker(List<T> checkList, Func<T, T, bool> checkFunc, float left, float right, float top, float bottom, float minimum, int nodePoolCount = 50, int density = 15)
        {
            _nodePool = new Stack<Node>(nodePoolCount);
            for (int ii = 0; ii < nodePoolCount; ++ii)
            {
                _nodePool.Push(new Node());
            }
            _checkList = checkList;

            _left = left;
            _right = right;
            _top = top;
            _bottom = bottom;
            _minimum = minimum;
            _density = density;

            _root = _nodePool.Pop();
            _root.SetRect(_left, _right, _top, _bottom);
            _checkFunc = checkFunc;
        }

        public void Update()
        {
            _root.Clear(_nodePool, false);

            foreach (var item in _checkList)
            {
                if (item.NeedCheck())
                {
                    _root.Push(item, _nodePool, _minimum, _density);
                }
            }
            _root.Check(_checkFunc);
        }
        public void Clear()
        {
            _nodePool.Clear();
            _checkList.Clear();
        }

        public void DrawGizmos()
        {
            if (_root != null)
            {
                _root.DrawGizmos();
            }
        }
    }
}