using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI.Xaml.Media;

namespace MasturbationRecorder {
    using TioSalamanca = List<IGrouping<BigInteger, StatistTotalByDateTime>>;

    class GusFring {
        private readonly Dictionary<string, SolidColorBrush> _gus = new Dictionary<string, SolidColorBrush>();
        private readonly IDictionary<int, SolidColorBrush> _colorDic;

        public GusFring(TioSalamanca[] tio, IDictionary<int, SolidColorBrush> colorDic) {
            _colorDic = colorDic;
            for (int level = 0; level < tio.Length; level++) {
                foreach (IGrouping<BigInteger, StatistTotalByDateTime> group in tio[level]) {
                    StatistTotalByDateTime[] res = group.ToArray();
                    foreach (var item in res) {
                        _gus.Add(item.DateTime.ToShortDateString(), colorDic[level + 1]);
                    }
                }
            }
        }

        public SolidColorBrush this[string rectName] {
            get {
                try {
                    return this._gus[rectName];
                }
                catch (KeyNotFoundException) {
                    return _colorDic[0];
                }
            }
        }
    }
}
