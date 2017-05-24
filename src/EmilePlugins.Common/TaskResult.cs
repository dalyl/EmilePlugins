using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmilePlugins.Common
{

    public class TaskResult
    {
        public TaskResult() { Succeeded = true; }

        public TaskResult(bool success)
        {
            Succeeded = success;
        }

        public TaskResult(TaskResult status)
        {
            AddErrors(status);
        }

        public TaskResult(IEnumerable<string> errors)
        {
            if (errors == null || errors.Count() == 0)
            {
                Succeeded = true;
            }
            else
            {
                AddErrors(errors);
            }
        }

        public TaskResult(params string[] errors)
        {
            if (errors == null)
            {
                Succeeded = true;
            }
            else
            {
                Succeeded = !(errors.Count() > 0);
                Errors = errors.ToList();
            }
        }

        public List<string> Errors { get; private set; }

        public TaskResult AddError(string error)
        {
            if (Errors == null)
                Errors = new List<string>();
            Errors.Add(error);
            Succeeded = false;
            return this;
        }

        public TaskResult AddErrors(IEnumerable<string> errors)
        {
            if (Errors == null)
                Errors = new List<string>();
            Errors.AddRange(errors);
            Succeeded = false;
            return this;
        }

        public TaskResult AddErrors<E>(Func<IEnumerable<E>> FindErrors,Func<E, string> FormatError)
        {
            if (Errors == null)  Errors = new List<string>();
            var colloction = FindErrors();
            return AddErrors(colloction, FormatError);
        }

        public TaskResult AddErrors<E>(IEnumerable<E> Colloction, Func<E, string> FormatError)
        {
            if (Errors == null) Errors = new List<string>();
            foreach (var one in Colloction)
            {
                var error = FormatError(one);
                Errors.Add(error);
            }
            Succeeded = false;
            return this;
        }

        public TaskResult AddErrors<C, E>(C Colloction, Func< E, string> FormatError)
            where C:CollectionBase
            where  E:class
        {
            if (Errors == null) Errors = new List<string>();
            foreach (var one in Colloction)
            {
                var error = FormatError(one as  E);
                Errors.Add(error);
            }
            Succeeded = false;
            return this;
        }

        

        public TaskResult AddErrors(TaskResult status)
        {
            if (status.Succeeded) return this;
            if (status.Errors == null) return this;
            if (status.Errors.Count == 0) return this;
            AddErrors(status.Errors);
            return this;
        }

        public bool Succeeded { get; private set; }

    }

    public class TaskResult<T> : TaskResult
    {
        public T Content { get; protected set; }

        public TaskResult<T> AddContent(T model)
        {
            Content = model;
            return this;
        }

        public TaskResult<T> AddContent(T model, TaskResult status)
        {
            Content = model;
            base.AddErrors(status);
            return this;
        }

        public void Copy(TaskResult<T> source)
        {
            this.AddContent(source.Content);
            this.AddErrors(source);
        }

        public TaskResult(TaskResult<T> source) : base(true)
        {
            Copy(source);
        }

        public TaskResult(TaskResult status)
        {
            AddErrors(status);
        }

       // [JsonConstructor]
        public TaskResult(bool success, T result, IEnumerable<string> errors) : base(errors)
        {
            Content = result;
        }

        public TaskResult(bool success, T result) : base(success)
        {
            Content = result;
        }

        public TaskResult(T result) : base(true)
        {
            Content = result;
        }

        public TaskResult(bool success) : base(success) { }

        public TaskResult(IEnumerable<string> errors) : base(errors) { }

        public TaskResult(params string[] errors) : base(errors) { }

        public TaskResult() : base() { }


        public new TaskResult<T> AddError(string error)
        {
            error = error.Replace("\n",string.Empty).Replace("\r", string.Empty);
            base.AddError(error);
            return this;
        }

        public new TaskResult<T> AddErrors(IEnumerable<string> errors)
        {
            base.AddErrors(errors);
            return this;
        }

        public new TaskResult<T> AddErrors(TaskResult status)
        {
            base.AddErrors(status);
            return this;
        }


        public new TaskResult<T> AddErrors<E>(Func<IEnumerable<E>> FindErrors, Func<E, string> FormatError)
        {
            base.AddErrors(FindErrors, FormatError);
            return this;
        }

        public new TaskResult<T> AddErrors<E>(IEnumerable<E> Colloction, Func<E, string> FormatError)
        {
            base.AddErrors(Colloction, FormatError);
            return this;
        }

        public new TaskResult<T> AddErrors<C, E>(C Colloction, Func<E, string> FormatError)
            where C : CollectionBase
            where E : class
        {
            base.AddErrors(Colloction, FormatError);
            return this;
        }


    }


}
