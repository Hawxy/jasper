﻿using System;
using System.Collections.Generic;
using System.Linq;
using Jasper.Http.Routing.Codegen;
using LamarCodeGeneration.Frames;

namespace Jasper.Http.Routing
{
    public class Spread : ISegment, IRoutingFrameSource
    {
        public Spread(int position)
        {
            Position = position;
        }

        public Frame ToParsingFrame(MethodCall action)
        {
            var parameter = action.Method.GetParameters().Single(x => x.IsSpread());
            return parameter.Name == JasperRoute.PathSegments
                ? (Frame) new PathSegmentsFrame(Position)
                : new RelativePathFrame(Position);
        }

        public int Position { get; }

        public string SegmentPath { get; } = "...";

        public string SegmentFromModel(object model)
        {
            throw new NotSupportedException();
        }

        public string ReadRouteDataFromMethodArguments(List<object> arguments)
        {
            throw new NotSupportedException();
        }

        public string SegmentFromParameters(IDictionary<string, object> parameters)
        {
            throw new NotSupportedException();
        }

        public string RoutePatternPath()
        {
            throw new NotImplementedException();
        }

        private string[] getSpreadData(string[] segments)
        {
            if (segments.Length == 0) return new string[0];

            if (Position == 0) return segments;

            if (Position > segments.Length - 1) return new string[0];

            return segments.Skip(Position).ToArray();
        }

        public override string ToString()
        {
            return $"spread:{Position}";
        }

        protected bool Equals(Spread other)
        {
            return Position == other.Position;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Spread) obj);
        }

        public override int GetHashCode()
        {
            return Position;
        }
    }
}
