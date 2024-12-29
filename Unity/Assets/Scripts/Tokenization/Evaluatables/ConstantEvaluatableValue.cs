namespace SpaceDeck.Tokenization.Evaluatables
{
    using SpaceDeck.Tokenization.Minimum;
    using SpaceDeck.GameState.Minimum;
    using SpaceDeck.GameState.Context;
    using System.Collections.Generic;
    using SpaceDeck.Tokenization.Minimum.Questions;
    using System;
    using SpaceDeck.Tokenization.Minimum.Context;

    /// <summary>
    /// Describes a very reliable answer to an evaluation question.
    /// It's always <see cref="Constant"/>.
    /// 
    /// This makes it able to answer some questions without any context that other
    /// evaluatable values would still need context for. This is reliable enough for it to be encouraged that programmers use
    /// "if (yourEvaluatable is ConstantEvaluatableValue<typeparamref name="T"/> constant) { ...  }"
    /// as a way to simplify flows.
    /// 
    /// <see cref="Constant"/> might be null, though.
    /// </summary>
    /// <typeparam name="T">The type this represents.</typeparam>
    public class ConstantEvaluatableValue<T> : IEvaluatableValue<T>
    {
        public readonly T Constant;

        public ConstantEvaluatableValue(T constant)
        {
            this.Constant = constant;
        }

        public IReadOnlyList<ExecutionQuestion> GetQuestions(LinkedToken linkedToken) => Array.Empty<ExecutionQuestion>();

        public bool TryEvaluate(ScriptingExecutionContext context, out T value)
        {
            value = this.Constant;
            return true;
        }

        public bool TryEvaluate(IGameStateMutator mutator, out T value)
        {
            value = this.Constant;
            return true;
        }

        public virtual string Describe()
        {
            return this.Constant.ToString();
        }
    }
}