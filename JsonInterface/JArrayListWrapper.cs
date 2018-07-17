using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonInterface.Extensions;
using JsonInterface.Handlers;
using Newtonsoft.Json.Linq;

namespace JsonInterface
{
    internal class JArrayListWrapper<T> : IJsonList<T>
    {
        private readonly IReadJsonTypeHandler<T> _readJsonTypeHandler;
        private readonly IWriteJsonTypeHandler<T> _writeJsonTypeHandler;
        private readonly JsonInterfaceSettings _settings;

        public JArrayListWrapper(JArray jArray, JsonInterfaceSettings settings)
        {
            JsonArray = jArray ?? throw new ArgumentNullException(nameof(JArray));
            _settings = settings.DefaultIfNull();

            _readJsonTypeHandler = HandlerFor<T>.GetReadJsonTypeHandler(_settings);
            _writeJsonTypeHandler = HandlerFor<T>.GetWriteJsonTypeHandler(_settings);

            // fail if type is faulted 
            _readJsonTypeHandler.ThrowIfFaulted();
        }

        public JArray JsonArray { get; set; }

        public T FromJToken(JToken jToken) => _readJsonTypeHandler.FromToken(jToken, _settings);

        public JToken ToJToken(T value) => _readJsonTypeHandler.ToToken(value, _settings);

        public T this[int index]
        {
            get => FromJToken(JsonArray[index]);
            set => JsonArray[index] = ToJToken(value);
        }

        public int Count => JsonArray.Count;

        public bool IsReadOnly => JsonArray.IsReadOnly;

        public virtual void Add(T item) => JsonArray.Add(ToJToken(item));

        public void Clear() => JsonArray.Clear();

        public virtual bool Contains(T item) => JsonArray.Any(v => JToken.DeepEquals(v, ToJToken(item)));

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int x = 0; x < Count; x++)
            {
                array[x] = this[arrayIndex + x];
            }
        }

        public IEnumerator<T> GetEnumerator() => CreateEnumerator();

        public int IndexOf(T item) => JsonArray.IndexOf(JsonArray.Where(v => JToken.DeepEquals(v, ToJToken(item))).FirstOrDefault());

        public void Insert(int index, T item) => JsonArray.Insert(index, ToJToken(item));

        public bool Remove(T item) => JsonArray.Remove(JsonArray.Where(v => JToken.DeepEquals(v, ToJToken(item))).FirstOrDefault());

        public void RemoveAt(int index) => JsonArray.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => CreateEnumerator();

        private IEnumerator<T> CreateEnumerator() => new JArrayListEnumerator<T>(this, JsonArray.GetEnumerator());

        /// <summary>
        /// Returns true if both objects are the same json object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            JArray otherObject = (obj as JArray) ?? (obj as JArrayListWrapper<T>)?.JsonArray;

            return JsonArray.Equals(otherObject);
        }

        /// <summary>
        /// Return the hash code for the json array object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => JsonArray.GetHashCode();

        public T AddNew()
        {
            var token = ToJToken(default(T));
            JsonArray.Add(token);
            return _readJsonTypeHandler.FromToken(token, _settings);
        }

        public T AddNew(Action<T> initializer)
        {
            var value = AddNew();
            initializer(value);
            return value;
        }

        public override string ToString() => JsonArray.ToString();
    }
}
