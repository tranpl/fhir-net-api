﻿/* 
 * Copyright (c) 2015, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/ewoutkramer/fhir-net-api/master/LICENSE
 */

using Hl7.Fhir.FluentPath;
using System;
using System.Linq;
using System.Collections.Generic;
using Hl7.Fhir.Support;
using Hl7.Fhir.FluentPath.Functions;

namespace Hl7.Fhir.FluentPath.Binding
{

    public class BindingTable
    {
        static BindingTable()
        {
            // Functions that operate on the focus, without null propagation
            add("empty", (IEnumerable<object> f) => !f.Any());
            add("exists", (IEnumerable<object> f) => f.Any());
            add("count", (IEnumerable<object> f) => f.Count());

            // Functions that use normal null propagation and work with the focus (buy may ignore it)
            nullp("not", (IEnumerable<IValueProvider> f) => f.Not());
            nullp("builtin.children", (IEnumerable<IValueProvider> f, string a) => f.Children(a));

            nullp("binary.=", (object f, IEnumerable<IValueProvider>  a, IEnumerable<IValueProvider> b) => a.IsEqualTo(b));
            nullp("binary.!=", (object f, IEnumerable<IValueProvider> a, IEnumerable<IValueProvider> b) => !a.IsEqualTo(b));

            nullp("unary.-", (object f, long a) => -a);
            nullp("unary.-", (object f, decimal a) => -a);
            nullp("unary.+", (object f, long a) => a);
            nullp("unary.+", (object f, decimal a) => a);

            nullp("binary.*", (object f, long a, long b) => a * b);
            nullp("binary.*", (object f, decimal a, decimal b) => a * b);

            nullp("binary./", (object f, decimal a, decimal b) => a / b);
            //.Add((object f, decimal a, decimal b) => a / b);

            nullp("binary.+", (object f, long a, long b) => a + b);
            nullp("binary.+", (object f, decimal a, decimal b) => a + b);
            nullp("binary.+", (object f, string a, string b) => a + b);

            nullp("binary.-", (object f, long a, long b) => a - b);
            nullp("binary.-", (object f, decimal a, decimal b) => a - b);

            nullp("binary.div", (object f, long a, long b) => a / b);
            nullp("binary.div", (object f, decimal a, decimal b) => (long)Math.Truncate(a / b));

            nullp("binary.mod", (object f, long a, long b) => a % b);
            nullp("binary.mod", (object f, decimal a, decimal b) => a % b);

            nullp("binary.>", (object f, long a, long b) => a > b);
            nullp("binary.>", (object f, decimal a, decimal b) => a > b);
            nullp("binary.>", (object f, string a, string b) => String.Compare(a, b) > 0);
            nullp("binary.>", (object f, PartialDateTime a, PartialDateTime b) => a > b);
            nullp("binary.>", (object f, Time a, Time b) => a > b);

            nullp("binary.<", (object f, long a, long b) => a < b);
            nullp("binary.<", (object f, decimal a, decimal b) => a < b);
            nullp("binary.<", (object f, string a, string b) => String.Compare(a, b) < 0);
            nullp("binary.<", (object f, PartialDateTime a, PartialDateTime b) => a < b);
            nullp("binary.<", (object f, Time a, Time b) => a < b);

            nullp("binary.<=", (object f, long a, long b) => a <= b);
            nullp("binary.<=", (object f, decimal a, decimal b) => a <= b);
            nullp("binary.<=", (object f, string a, string b) => String.Compare(a, b) <= 0);
            nullp("binary.<=", (object f, PartialDateTime a, PartialDateTime b) => a <= b);
            nullp("binary.<=", (object f, Time a, Time b) => a <= b);

            nullp("binary.>=", (object f, long a, long b) => a >= b);
            nullp("binary.>=", (object f, decimal a, decimal b) => a >= b);
            nullp("binary.>=", (object f, string a, string b) => String.Compare(a, b) >= 0);
            nullp("binary.>=", (object f, PartialDateTime a, PartialDateTime b) => a >= b);
            nullp("binary.>=", (object f, Time a, Time b) => a >= b);

            nullp("binary.|", (object f, IEnumerable<IValueProvider> l, IEnumerable<IValueProvider> r) => l.Union(r) );
            nullp("binary.contains", (object f, IEnumerable<IValueProvider> a, IValueProvider b) => a.Contains(b) );
            nullp("binary.in", (object f, IValueProvider a, IEnumerable<IValueProvider> b) => b.Contains(a));
            nullp("distinct", (IEnumerable<IValueProvider> f) => f.Distinct());
            nullp("isDistinct", (IEnumerable<IValueProvider> f) => f.IsDistinct());
            nullp("subsetOf", (IEnumerable<IValueProvider> f, IEnumerable<IValueProvider> a) => f.SubsetOf(a));
            nullp("supersetOf", (IEnumerable<IValueProvider> f, IEnumerable<IValueProvider> a) => a.SubsetOf(f));

            nullp("single", (IEnumerable<IValueProvider> f) => f.Single());
            nullp("skip", (IEnumerable<IValueProvider> f, long a) =>  f.Skip((int)a));
            nullp("first", (IEnumerable<IValueProvider> f) => f.First());
            nullp("last", (IEnumerable<IValueProvider> f) => f.Last());
            nullp("tail", (IEnumerable<IValueProvider> f) => f.Tail());
            nullp("take", (IEnumerable<IValueProvider> f, long a) => f.Take((int)a));
            nullp("builtin.item", (IEnumerable<IValueProvider> f, long a) => f.Item((int)a));

            nullp("toInteger", (IValueProvider f) => f.ToInteger());
            nullp("toDecimal", (IValueProvider f) => f.ToDecimal());
            nullp("toString", (IValueProvider f) => f.ToStringRepresentation());

            nullp("substring", (string f, long a) => f.Substring((int)a));
            nullp("substring", (string f, long a, long b) => f.Substring((int)a, (int)b));

            nullp("today", (object f) => PartialDateTime.Today());
            nullp("now", (object f) => PartialDateTime.Now());

            // Logic operators do not use null propagation and may do short-cut eval
            logic("binary.and", (a, b) => a.And(b));
            logic("binary.or", (a, b) => a.Or(b));
            logic("binary.xor", (a, b) => a.XOr(b));
            logic("binary.implies", (a, b) => a.Implies(b));

            // Special late-bound functions
            _functions.Add(new CallBinding("where", buildWhereLambda(), typeof(object), typeof(Evaluator)));
        }

        public static Invokee Resolve(string functionName, IEnumerable<Type> argumentTypes)
        {
            var bindings = _functions.Where(f => f.StaticMatches(functionName, argumentTypes));                        

            if(bindings.Any())
            {
                if (bindings.Count() > 1)
                {
                    return (new DynaDispatcher(functionName, bindings)).Invoke();
                }
                else
                    return bindings.Single().Function;
            }

            else
            {
                if (_functions.Any(f => f.Name == functionName))
                {
                    // No function could be found, but there IS a function with the given name, 
                    // report an error about the fact that the function is known, but could not be bound
                    throw Error.Argument("Function '{0}' is not called with the right number or type of parameters".FormatWith(functionName));
                }
                else
                {
                    // Not an internally known function, forward to context (so it can provide a hook to handle it)
                    return buildExternalCall(functionName);
                }
            }
        }


        private static List<CallBinding> _functions = new List<CallBinding>();


        private static void add<A,R>(string name, Func<A, R> focusFunc)
        {
            _functions.Add(new CallBinding(name, InvokeeFactory.Wrap(focusFunc), typeof(A)));
        }

        private static void nullp<F>(string name, Func<F,object> func)
        {
            _functions.Add(new CallBinding(name, InvokeeFactory.Wrap(func).NullProp(), typeof(F)));
        }

        private static void nullp<F,A>(string name, Func<F,A,object> func)
        {
            _functions.Add(new CallBinding(name, InvokeeFactory.Wrap(func).NullProp(), typeof(F), typeof(A)));
        }

        private static void nullp<F,A, B>(string name, Func<F,A,B,object> func)
        {
            _functions.Add(new CallBinding(name, InvokeeFactory.Wrap(func).NullProp(), typeof(F), typeof(A), typeof(B)));
        }

        private static void logic(string name, Func<Func<bool?>,Func<bool?>,bool?> func)
        {
            _functions.Add(new CallBinding(name, InvokeeFactory.WrapLogic(func), typeof(object), typeof(Func<bool?>), typeof(Func<bool?>)));
        }

        private static Invokee buildExternalCall(string name)
        {
            return (ctx, args) =>
            {
                var focus = ctx.GetThis();
                var evaluatedArguments = args.Select(a => a(ctx));
                return ctx.InvokeExternalFunction(name, focus, evaluatedArguments);
            };
        }

        private static Invokee buildWhereLambda()
        {
            return (ctx, args) =>
            {
                var focus = ctx.GetThis();
                Evaluator lambda = args.First();

                return run(ctx, focus, lambda);
            };
        }
        

        private static IEnumerable<IValueProvider> run(IEvaluationContext ctx, IEnumerable<IValueProvider> focus, Evaluator lambda)
        {
            foreach (IValueProvider element in focus)
            {
                var newContext = ctx.Nest(FhirValueList.Create(element));
                if (lambda(newContext).BooleanEval() == true)
                    yield return element; 
            }
        }
    }
}