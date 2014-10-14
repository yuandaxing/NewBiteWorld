using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingIntent.ExpMod
{
    public class SemiMarkov : FBTable
    {
        public SemiMarkov(ExpModel model) :
            base(model)
        {
            featureList = new FeatureCountList(model);
        }

        private FeatureCountList featureList;

        public override double Forward(bool markedState, LabelSequence labelSequence)
        {
            int[] allowStates = factory.GetAllAllowedStates();
            GetInitCell().Alpha = 0.0;
            int[] states = (labelSequence == null) ? sample.GetStates() : labelSequence.States.ToArray();

            for (int iframe = 0; iframe <= sample.GetSize(); iframe++)
            {
                int iToStateStart = 0, iToStateEnd = allowStates.Length;
                int endMin = iframe + 1, endMax = sample.GetSize();
                if (markedState)
                {
                    if (labelSequence != null && !labelSequence.IsBoundary(iframe) ||
                        !sample.IsBoundary(iframe))
                        continue;
                    iToStateStart = iframe < sample.GetSize() ?
                        factory.GetStateIdx(states[iframe]) : GetFinalCell().StateIdx;
                    iToStateEnd = iToStateStart + 1;
                    for (; endMin <= endMax; endMin++)
                        if (sample.IsBoundary(endMin))
                            break;
                    endMax = endMin;
                }
                for (; iToStateStart < iToStateEnd; iToStateStart++)
                {
                    int[] trans = factory.GetPreviousStateTable()[iToStateStart];
                    FBCell toCell = GetCell(iframe, iToStateStart);
                    int toStateID = allowStates[iToStateStart];

                    for (int jFromStateStart = 0; jFromStateStart < trans.Length; jFromStateStart++)
                    {
                        int fromIdx = trans[jFromStateStart];
                        int fromStateID = allowStates[fromIdx];
                        FBCell fromCell = GetCell(iframe, fromIdx);
                        if (fromCell.Alpha != Ln.lnZero)
                        {
                            featureList.Clear();
                            factory.RetrieveFeatureList(featureList, sample, iframe,
                                iframe, fromStateID, toStateID);
                            toCell.InComingAlpha = Ln.Add(toCell.InComingAlpha,
                                Ln.Mult(fromCell.Alpha, featureList.Score));
                        }
                    }
                    //expand post command is OK
                    if (iframe < sample.GetSize() && toCell.InComingAlpha != Ln.lnZero
                        && toCell.StateIdx != GetInitCell().StateIdx)
                    {
                        int endFrame = endMin;
                        while (endFrame <= endMax)
                        {
                            featureList.Clear();
                            factory.RetrieveFeatureList(featureList, sample, iframe, endFrame, toStateID,
                                toStateID);
                            FBCell endCell = GetCell(endFrame, iToStateStart);
                            endCell.Alpha = Ln.Add(endCell.Alpha,
                                Ln.Mult(featureList.Score, toCell.InComingAlpha));
                            endFrame++;
                        }
                    }
                }
            }
            return GetFinalCell().InComingAlpha;
        }


        public override double Backward(bool markedState, LabelSequence labelSequence)
        {
            int[] allowStates = factory.GetAllAllowedStates();
            GetFinalCell().Beta = 0.0;
            int[] states = (labelSequence == null) ? sample.GetStates() : labelSequence.States.ToArray();

            for (int iframe = sample.GetSize(); iframe >= 0; iframe--)
            {
                int iFromStateStart = 0, iFromStateEnd = allowStates.Length;
                int endMin = 0, endMax = iframe - 1;
                if (markedState)
                {
                    if (labelSequence != null && !labelSequence.IsBoundary(iframe) ||
                        !sample.IsBoundary(iframe))
                        continue;
                    iFromStateStart = iframe > 0 ?
                        factory.GetStateIdx(states[iframe - 1]) : GetInitCell().StateIdx;
                    iFromStateEnd = iFromStateStart + 1;
                    for (endMin = iframe - 1; endMin >= 0; endMin--)
                        if (sample.IsBoundary(endMin))
                            break;
                    endMax = endMin;
                }
                for (; iFromStateStart < iFromStateEnd; iFromStateStart++)
                {
                    int[] trans = factory.GetNextStateTable()[iFromStateStart];
                    FBCell fromCell = GetCell(iframe, iFromStateStart);
                    int fromStateID = allowStates[iFromStateStart];

                    for (int jToStateStart = 0; jToStateStart < trans.Length; jToStateStart++)
                    {
                        int toStateIdx = trans[jToStateStart];
                        int toStateID = allowStates[toStateIdx];
                        FBCell toCell = GetCell(iframe, toStateIdx);
                        if (toCell.Beta != Ln.lnZero)
                        {
                            featureList.Clear();
                            factory.RetrieveFeatureList(featureList, sample, iframe,
                                iframe, fromStateID, toStateID);
                            fromCell.InComingBeta = Ln.Add(fromCell.InComingBeta,
                                Ln.Mult(toCell.Beta, featureList.Score));
                        }
                    }
                    //expand post command is OK
                    if (iframe > 0 && fromCell.InComingBeta != Ln.lnZero
                        && fromCell.StateIdx != GetFinalCell().StateIdx)
                    {
                        int endFrame = endMin;
                        while (endFrame <= endMax)
                        {
                            featureList.Clear();
                            factory.RetrieveFeatureList(featureList, sample, endFrame, iframe, fromStateID, fromStateID);
                            FBCell endCell = GetCell(endFrame, iFromStateStart);
                            endCell.Beta = Ln.Add(endCell.Beta,
                                Ln.Mult(featureList.Score, fromCell.InComingBeta));
                            endFrame++;
                        }
                    }
                }
            }
            return GetInitCell().InComingBeta;
        }

        public override void CollectFeatureCount(AccumulatedFeatureCountList featureCounts,
             double totalLnScore, bool markedState, LabelSequence labelSequence)
        {
            int[] allowStates = factory.GetAllAllowedStates();
            int[] states = (labelSequence == null) ? sample.GetStates() : labelSequence.States.ToArray();

            for (int iframe = 0; iframe <= sample.GetSize(); iframe++)
            {
                int iToStateStart = 0, iToStateEnd = allowStates.Length;
                int endMin = iframe + 1, endMax = sample.GetSize();
                if (markedState)
                {
                    if (labelSequence != null && !labelSequence.IsBoundary(iframe) ||
                        !sample.IsBoundary(iframe))
                        continue;
                    iToStateStart = iframe < sample.GetSize() ?
                        factory.GetStateIdx(states[iframe]) : GetFinalCell().StateIdx;
                    iToStateEnd = iToStateStart + 1;
                    for (; endMin <= endMax; endMin++)
                        if (sample.IsBoundary(endMin))
                            break;
                    endMax = endMin;
                }
                for (; iToStateStart < iToStateEnd; iToStateStart++)
                {
                    int[] trans = factory.GetPreviousStateTable()[iToStateStart];
                    FBCell toCell = GetCell(iframe, iToStateStart);
                    int toStateID = allowStates[iToStateStart];

                    for (int jFromStateStart = 0; jFromStateStart < trans.Length; jFromStateStart++)
                    {
                        int fromStateIdx = trans[jFromStateStart];
                        int fromStateID = allowStates[fromStateIdx];
                        FBCell fromCell = GetCell(iframe, fromStateIdx);

                        if (fromCell.Alpha != Ln.lnZero && toCell.Beta != Ln.lnZero)
                        {
                            featureList.Clear();
                            factory.RetrieveFeatureList(featureList, sample, iframe,
                                iframe, fromStateID, toStateID);
                            double gamma = Ln.Mult(Ln.Mult(fromCell.Alpha, toCell.Beta),
                                featureList.Score);
                            gamma = Ln.Exp(Ln.Div(gamma, totalLnScore));
                            for (int i = 0; i < featureList.Size; i++)
                            {
                                featureCounts.Add(featureList.GetFeatureID(i), gamma);
                            }
                        }
                    }
                    //expand post command is OK
                    if (iframe < sample.GetSize() && toCell.InComingAlpha != Ln.lnZero
                        && toCell.StateIdx != GetInitCell().StateIdx && toCell.StateIdx !=
                        GetFinalCell().StateIdx)
                    {
                        int endFrame = endMin;
                        while (endFrame <= endMax)
                        {
                            FBCell endCell = GetCell(endFrame, iToStateStart);
                            if (endCell.InComingBeta != Ln.lnZero)
                            {
                                featureList.Clear();
                                factory.RetrieveFeatureList(featureList, sample, iframe,
                                    endFrame, toStateID, toStateID);
                                double gamma = Ln.Mult(featureList.Score, Ln.Mult(toCell.InComingAlpha,
                                    endCell.InComingBeta));
                                gamma = Ln.Exp(Ln.Div(gamma, totalLnScore));
                                for (int i = 0; i < featureList.Size; i++)
                                {
                                    featureCounts.Add(featureList.GetFeatureID(i), gamma);
                                }
                            }
                            endFrame++;
                        }
                    }
                }
            }
        }

        public override void ViterbiSearch(ISample sample)
        {
            int[] allowStates = factory.GetAllAllowedStates();
            int nbest = factory.GetNBest();
            GetInitCell().AddNBestTrace(GetInitCell(), null, 0.0, nbest);
            for (int iframe = 0; iframe <= sample.GetSize(); iframe++)
            {
                int iToStateStart = 0, iToStateEnd = allowStates.Length;
                int endMin = iframe + 1, endMax = sample.GetSize();

                for (; iToStateStart < iToStateEnd; iToStateStart++)
                {
                    int[] trans = factory.GetPreviousStateTable()[iToStateStart];
                    FBCell toCell = GetCell(iframe, iToStateStart);
                    int toStateID = allowStates[iToStateStart];

                    for (int jFromStateStart = 0; jFromStateStart < trans.Length; jFromStateStart++)
                    {
                        int fromStateIdx = trans[jFromStateStart];
                        int fromStateID = allowStates[fromStateIdx];
                        FBCell fromCell = GetCell(iframe, fromStateIdx);

                        if (fromCell.BestOutgoingScore != Ln.lnZero)
                        {
                            featureList.Clear();
                            factory.RetrieveFeatureList(featureList, sample, iframe,
                                iframe, fromStateID, toStateID);

                            toCell.AddNBestTrace(toCell, fromCell, featureList.Score,
                                nbest);
                        }
                    }
                    //expand post command is OK
                    if (iframe < sample.GetSize() && toCell.BestIncomingScore != Ln.lnZero
                        && toCell.StateIdx != GetInitCell().StateIdx)
                    {
                        int endFrame = endMin;
                        while (endFrame <= endMax)
                        {
                            FBCell endCell = GetCell(endFrame, iToStateStart);
                            featureList.Clear();
                            factory.RetrieveFeatureList(featureList, sample, iframe,
                                endFrame, toStateID, toStateID);
                            endCell.AddNBestTrace(endCell, toCell, featureList.Score,
                                nbest);
                            endFrame++;
                        }
                    }
                }
            }
        }
    }
}
